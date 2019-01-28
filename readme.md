# Cosmos Change Feed Issue Repro

Open the ChangeFeedConsole and restore Nuget packages--Uses the local Cosmos emulator 
	*Database Name: Sample
	*Collection Names: _Monitored, _Leases
	
## Repro instructions
	*Add a couple of documents with an id that does NOT contain the word "error" and notice these process normally