# Cosmos Change Feed Issue Repro

Open the CosmosChFd.sln and restore Nuget packages

**This sample uses the local Cosmos emulator, see FeedService.cs**

*Database Name: Sample _(can be overriden in FeedService.cs ctor)_
*Collection Names: _Monitored, _Leases _(can be overriden in FeedService.cs ctor)_
	
## Repro instructions
**Notice on start up we have the usual starting detail**
```
Starting processor...
The store is initialized
Acquired lease for PartitionId '0' on startup.
partition 0: acquired
SampleObserverFactory: Creating observer
SampleObserver::OpenAsync for 0
SampleObserver::ProcessChangesAsync: partition 0, 1 docs
Partition 0: renewer task started.
```
**Add a couple of documents with an id that does NOT contain the word "error" and notice these process normally**
*(you might get a lease update conflict, but it appears to resolve itself every time)*

**Now add a document with an id that contains "error" and notice the exception is thrown, processor and partition load balancer stop, but..**
```
SampleObserver::ProcessChangesAsync: partition 0, 1 docs
SampleProcessor: About to OnNext 
[[ exception occurs here ]]
Stopping processor...
Partition load balancer task stopped.
Partition 0: renewed lease with result True
Partition 0: renewed lease with result True
Partition 0: renewed lease with result True
.......... etc.
```
*The problem is the lease now appears to be in a strange "zombie" state where it thinks all is well, but it won't receive any more documents. You can add another doc and it will do nothing except continue to output the "renewed lease" message.*
	
	