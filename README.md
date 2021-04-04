
Hi!

I'm looking for help in creating a service that that will encompass hundreds of containerized AgentRunner instances, each hosting one or more agents (via plugins, although a detail omitted in this example) and deployed / managed via an AgentManager Function App employing one or more HttpTriggers.  

**DELIVERABLES**:

Add a **DeployAndConfigureInstance** function to the AgentManager that
- Creates a new pod on an Azure Kubernetes Service (AKS) instance
- Deploys a single Linux container to the new pod
  - The containers will be kept in a single Azure Container Repository instance
  - Each container will have a single AgentRunner running a single "agent" plugin
- Each pod will have a replica count of 1
  - There will be no need for auto-scaling, or even public endpoints
- All Kubernetes interaction will be mediated by the Kubernetes C# API
  - https://github.com/kubernetes-client/csharp
  - Scripting via kubectl of otherwise may NOT be employed!!
- Inline with the deployment process, each container will need to have a collection of environment variables set
  - These environment variables will be used by the AgentRunner app to connect to databases, Service Bus, APIs, etc.
  
Add a **DeleteInstance** function to AgentManager that
- Deletes an existing pod and container, but only after a graceful shutdown of the AgentRunner via an Azure Service Bus "Stop" message

Add a **ListInstances** function to AgentManager that
- Lists all of the "agent" pods + containers within a Kubernetes cluster

**DEMO**

The AgentDemo solution should serve as a good starting point for this project.  To get it up and running, you'll need to do a few things, though:

Create a "Settings.env" file in the AgentRunner folder, with the following Key=Value pairs

|Key|Value |
|----------|----------|
|ServiceBusConnString |{AZURE SERVICE BUS CONNECTION STRING}|
|TopicName|AAAA|
|SubscriptionName|AAAA-0001|

You'll also need to create an Azure Service Bus topic and subscription that matches the above values.  Once configured, launch the AgentRunner and AgentManager then post messages via REST to the AgentManager "Manager" method (at http://localhost:7071/api/Manager) to send commands to individual AgentRunner instances, via the AgentManager's Manager function; to Pause, Resume, Stop and Configure the service.

When quoting a price for the above work, please make provision for a quick kick-off meeting, via Zoom, to make sure we're both on the same page.

Looking forward to hearing from you...
