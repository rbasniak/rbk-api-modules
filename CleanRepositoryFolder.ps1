

Get-ChildItem -Path C:\git\Development\Demos\rbkApiModules\ -include bin,obj -recurse | remove-item -force -recurse
rem Get-ChildItem -Path C:\git\Development\Demos\rbkApiModules\ -include .vs -recurse -Hidden | remove-item -force -recurse