@echo off
@echo COMMAND-LINE BUILD FOR 
@echo Pro WF: Windows Workflow in .NET 4
@echo by Bruce Bukovics * Apress
rem ------------------------------------------------------------
set BUILDUTILITY=msbuild /verbosity:minimal 
set BUILDACTION=/t:Rebuild
set BUILDPROFILE=/p:Configuration=Debug /p:Platform="Any CPU"
rem -------------------------------------------------
if not "%1" == "" set BUILDACTION=%1
@echo BUILDACTION = %BUILDACTION%
pause
rem -------------------------------------------------
@echo START OF NEW BUILD > buildlog.txt
rem -------------------------------------------------
@echo .NET 4 / VISUAL STUDIO 2010 BUILDS
@echo BUILDING CHAPTER 1
%BUILDUTILITY% "chapter 01\chapter 01.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 2
%BUILDUTILITY% "chapter 02\chapter 02.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 3
%BUILDUTILITY% "chapter 03\chapter 03.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 4
%BUILDUTILITY% "chapter 04\chapter 04.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 5
%BUILDUTILITY% "chapter 05\chapter 05.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 6
%BUILDUTILITY% "chapter 06\chapter 06.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 7
%BUILDUTILITY% "chapter 07\chapter 07.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 8
%BUILDUTILITY% "chapter 08\chapter 08.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 9
%BUILDUTILITY% "chapter 09\chapter 09.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 11
%BUILDUTILITY% "chapter 11\chapter 11.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 13
%BUILDUTILITY% "chapter 13\chapter 13.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 14
%BUILDUTILITY% "chapter 14\chapter 14.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 15
%BUILDUTILITY% "chapter 15\chapter 15.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 16
%BUILDUTILITY% "chapter 16\chapter 16.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 17
%BUILDUTILITY% "chapter 17\chapter 17.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
@echo BUILDING CHAPTER 18
%BUILDUTILITY% "chapter 18\chapter 18.sln" %BUILDACTION% %BUILDPROFILE% >> buildlog.txt
rem -------------------------------------------------
:exit
@echo ------------------------------------------------ >> buildlog.txt
@echo ------- END OF BUILD - SUMMARY FOLLOWS --------->> buildlog.txt
@echo ------------------------------------------------ >> buildlog.txt
find "error" buildlog.txt >> buildlog.txt
rem -------------------------------------------------
start notepad.exe buildlog.txt
rem -------------------------------------------------
