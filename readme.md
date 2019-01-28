# Cosmos Change Feed Issue Repro

Open the ChangeFeedConsole and restore Nuget packages--Uses the local Cosmos emulator 
	*Database Name: Sample
	*Collection Names: _Monitored, _Leases
	
## Repro instructions
	*Notice on start up the normal info logging provides all the starting detail
		```Starting processor...
		The store is initialized
		Acquired lease for PartitionId '0' on startup.
		partition 0: acquired
		SampleObserverFactory: Creating observer
		SampleObserver::OpenAsync for 0
		SampleObserver::ProcessChangesAsync: partition 0, 1 docs
		Partition 0: renewer task started.```
	*Add a couple of documents with an id that does NOT contain the word "error" and notice these process normally
	