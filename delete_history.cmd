For /f "tokens=1-3 delims=. " %%a in ('date /t') do (set mydate=%%c%%b%%a)
FOR /F "tokens=2-2 delims= " %%a in ('git branch') do (set branch=%%a)

git checkout --orphan tmp-master
git add -A
git commit -m "Commit %mydate%"
git branch -D %branch% 
git branch -m %branch% 
git push -f origin %branch%
