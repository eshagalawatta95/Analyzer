# Analyzer
Analyzer Back-door Detection Tool.Analyze and detect back-doors in .exe files.

System Requirements,
1. Windows 7 or higher
2. Microsoft SQL server 14 or higher
3. .net framework 4.5 or higher
4. 1 GB RAM
5. 100 MB space
6. Active Internet connection

How to build

1. Clone Analyzer to your PC
2. Open project in Visual studio 17 or higher
3. Restore Detection matrix file where in App_data to MS sql server and change connection string as yours
4. Add your VirusTotal and Whois data API keys to "APIkey.txt" file
5. Create folder structure accordng to source code in mainform
6. Starting develop

Used languages
1. C# for core of Analyzer
2. C++ for plugins
3. SQL for Detection Matrix

Suggeted Future Developments

1. Integrate Analyzer with Machine Learning model to identifying backdoor process. It can get more accuracy than Detection Matrix method.
2. Proposed to upgrade tool in next development for capability to detect packed and obfuscated executables.
3. Proposed to develop multiplatform tool.
4. Suggest to expand supported file types. eg. .dll,.sys, etc.
5. Expand to Dynamic Analysis Techniques

Contact esh.agalawatta@gmail.com or https://stackoverflow.com/users/8200840/esh-harshana for more infomation.