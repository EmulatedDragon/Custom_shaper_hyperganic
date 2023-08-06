# CustomShapers 
This repo contains starter custom shaper tutorial app for curriculum purposes. 

## Best Practices (Workflow)
- Make sure you are using the most updated HyperganicCore. You may obtain this download from app.hyperganic.com 
- If you are using Windows:
1. try to make sure that this file is located in your C drive's download folder.
2. If you are facing error codes, it may be due to access permissions. Do make sure to check which user account your starter files are located in (check "C:\Users"). Preferably you would use one with admin access.
- If you are using a Mac:
1. Try to make sure your file is located in your downloads folder. 
2. Alternatively, you might need to adjust your directory in your .csproj. You can check the number of "..\" before "Applications". 

		<Compile Condition="$(OS) != 'Windows_NT'" Include="..\..\..\..\../Applications/HyperganicCore.app/Contents/Resources/CSharp/**/*.cs">
			<Link>Resources\CSharp\%(RecursiveDir)%(FileName)%(Extension)</Link>
		</Compile> 
- If you are facing other issues, please check 
https://forum.hyperganic.com/t/megathread-setup-troubleshooting-guide/320