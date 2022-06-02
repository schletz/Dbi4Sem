import ldap3  # pip install ldap3
import os, datetime as dt
import pandas as pd
import numpy as np

def create_client(username, password):
    server = ldap3.Server("ldap.spengergasse.at", port=636, use_ssl=True, get_info='ALL')
    return Ad_Client(username, password, server)


class Ad_Client:
    def __init__(self, username, password, server):
        self._user = username
        self._pass = password
        self._server = server

    def get_entries(self, filter='(objectClass=person)', attributes=['cn', 'mail'], base_dn = "OU=Benutzer,OU=SPG,DC=htl-wien5,DC=schule"):
        """
        Liest das Active Directory der Schule und gibt die gefundenen Einträge als Dataframe zurück.
        :param username: Der Benutzername für das Binding, z. B. ABC123456.
        :param password: Das entsprechende Passwort.
        :param filter: Ein LDAP Suchfilter.
        :param attributes: Die zu lesenden Attribute.
        :return: Ein Dataframe mit den Spalten dn und eine Spalte pro Attribut.
        """
        # See https://ldap3.readthedocs.io/en/latest/searches.html#simple-paged-search
        with ldap3.Connection(self._server, user=f'{self._user}@htl-wien5.schule', password=self._pass) as conn:
            # Der Server liefert max. 1000 Einträge. Wir machen also eine paged search mit 512 Einträgen.
            if conn.search(base_dn, filter,
                        search_scope=ldap3.SUBTREE, attributes=attributes, paged_size=512):
                cookie = conn.result['controls']['1.2.840.113556.1.4.319']['value']['cookie']
                results = pd.DataFrame(map(lambda val: val.get("attributes"), conn.response))
                while cookie:
                    # Die nächsten Seiten abfragen.
                    conn.search(base_dn, filter,
                                search_scope=ldap3.SUBTREE, attributes=attributes, paged_size=512,
                                paged_cookie=cookie)
                    result = pd.DataFrame(map(lambda val: val.get("attributes"), conn.response))
                    results = pd.concat([results, result])
                    cookie = conn.result['controls']['1.2.840.113556.1.4.319']['value']['cookie']
            else:
                raise RuntimeError("Fehler beim Durchsuchen des AD.")
            return results

    def read_ad(self):
        """
        Liest Grunddaten in einen Dataframe und bereitet die Daten auf. Dabei werden Listen, die
        nur ein element beinhalten (also [XX] stat XX) mit explode auf den Inhalt gesetzt.
        Timestamps aus dem Jahr 1600 oder 9999 (für unbegrenzt) werden korrigiert, damit diese
        gespeichert werden können.
        Das Lesen des ganzen AD braucht ein paar Sekunden. Daher schreiben wir eine Datei
        ad_user.parquet als Cache. Ist sie vom selben Tag, wird diese Datei geliefert.
        """
        if os.path.isfile("ad_user.parquet") and dt.date.fromtimestamp(os.path.getmtime("ad_user.parquet")) == dt.date.today():
            return pd.read_parquet("ad_user.parquet")
        mappings = {
            "givenName": "FIRSTNAME", "sn": "LASTNAME", "mail": "MAIL", "cn": "CN", "employeeID": "PUPIL_ID", "description": "TEACHER_ID",
            "memberOf": "MEMBER_OF", "lastLogon": "LAST_LOGON", "lastLogonTimestamp": "LAST_LOGON_TS", "accountExpires": "ACCOUNT_EXPIRES", "whenCreated": "CREATED", "whenChanged": "CHANGED"}
        ad_user = self.get_entries(attributes=mappings.keys()) \
            .explode("givenName") \
            .explode("sn") \
            .explode("mail") \
            .explode("employeeID") \
            .explode("description") \
            .explode("lastLogonTimestamp") \
            .explode("accountExpires") \
            .explode("whenCreated") \
            .explode("whenChanged") \
            .assign(employeeID = lambda df: df.employeeID.where(df.employeeID.str.match("^\d+$"), np.nan).astype("Int64")) \
            .assign(description = lambda df: df.description.where(df.description.str.match("^[A-Z]+$"), np.nan)) \
            .assign(lastLogon = lambda df: pd.to_datetime(df.lastLogon, errors="coerce", utc=True)) \
            .assign(lastLogonTimestamp = lambda df: pd.to_datetime(df.lastLogonTimestamp, errors="coerce", utc=True)) \
            .assign(accountExpires =lambda df: pd.to_datetime(df.accountExpires, errors="coerce", utc=True)) \
            .assign(whenCreated = lambda df: pd.to_datetime(df.whenCreated, errors="coerce", utc=True)) \
            .assign(whenChanged = lambda df: pd.to_datetime(df.whenChanged, errors="coerce", utc=True)) \
            .assign(lastLogon = lambda df: df[["lastLogon", "lastLogonTimestamp"]].max(axis=1, numeric_only=False)) \
            .rename(mappings, axis=1) \
            .drop("LAST_LOGON_TS", axis=1) \
            .set_index("CN")
        ad_user.to_parquet("ad_user.parquet")
        return ad_user