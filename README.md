# XivReClassPlugin
ReClass.NET Plugin to make use of the info gathered in [FFXIVClientStructs](https://github.com/aers/FFXIVClientStructs)

just throw the dll into ```ReClass.Net\x64\Plugins\``` and set the path to ```FFXIVClientStructs\ida\data.yml``` in the settings.

* simulates having RTTI info available with the things defined in data.yml
* shows what class a pointer points at if available
* adds a slightly improved C# Code Generator to generate strucs for FFXIVClientStructs
* resolves instance names to addresses where available (e.g. `<Client::Game::Object::GameObjectManager_Instance>`)
* resolves Addon addresses with `<Addon(id)>` or `<Addon(name)>`
* resolves Agent addresses with `<Agent(id)>` or `<Agent(name)>` if the name is defined in data.yml
* adds a live list of all agents that shows their ID, Size, defined Class name in data.yml and the name of the Addon that's currently using it
