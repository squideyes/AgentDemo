Dear Freelancer:

I'm looking for help in creating an Azure-deployed SaaS offering that that will encompass hundreds of containerized AgentRunner instances (see the repo at  [https://github.com/squideyes/AgentDemo](https://github.com/squideyes/AgentDemo) for starter code and documentation), each hosting one or more internal "agents" and deployed to / managed in Azure Kubernetes Service (AKS) via an AgentManager Function App employing one or more HttpTriggers.  

As a pre-requisite for this work, you'll need a paid or [free](https://azure.microsoft.com/en-us/free/) Azure account with:

* **Azure Container Repository** (hosts AgentRunner images)
* **Azure Kubernetes Service** (AKS) (orchestrates AgentRunner containers)
* **Azure Function App** (hosts AgentManager Function App, which configures/manages AKS)
* **Service Bus Topic** (communicates with individual AgentRunner container instances)

**DELIVERABLES** are  the provided AgentDemo solution updated as follows:

Add a **DeployAndConfigureInstance** function to the AgentManager Function App that
- Creates a new pod on AKS
- Deploys a single Linux container to the new pod
  - The container images will be kept in a single Azure Container Repository
  - Each container will host a single AgentRunner running a single "agent"
- Each pod will have a replica count of 1
  - There will be no need for auto-scaling, or even public HTTP endpoints
- All Kubernetes interaction will be mediated by the Kubernetes C# API
  - https://github.com/kubernetes-client/csharp
  - Scripting via kubectl or otherwise may NOT be employed!!
- Inline with the deployment process, **each container will need to have a collection of environment variables set**
  - These pod-specific environment variables will be used by the containerized AgentRunner instances to connect to databases, Service Bus, APIs, etc.
  
Add a **DeleteInstance** function to the AgentManager Function App that
- Deletes an existing pod and container, but only after a graceful shutdown of the AgentRunner via an Azure Service Bus "Stop" message

Add a **ListInstances** function to the AgentManager Function App that
- Lists all of the "agent" pods + containers within a Kubernetes cluster

**DEMO**

The **AgentDemo** solution should serve as a good starting point for this project.  To get it up and running, you'll need to do a few things, though:

Create a "Settings.env" file in the AgentRunner folder, with the following Key=Value pairs

|Key|Value |
|----------|----------|
|ServiceBusConnString |{AZURE SERVICE BUS CONNECTION STRING}|
|TopicName|AAAA|
|SubscriptionName|AAAA-0001|

You'll also need to **create an Azure Service Bus topic and subscription** that matches the above values.  Once configured, launch both the AgentRunner and AgentManager then post messages via REST to the AgentManager "Manager" method (by default, at http://localhost:7071/api/Manager) to send commands to individual AgentRunner container instances, via the AgentManager's Manager function; to Pause, Resume, Stop and Configure the service.

To interact with the AgentManager FunctionApp, use an app like Postman to issue commands similar to the following:

```json
{
    "Kind": "Resume",
    "TraderId": "AAAA",
    "AccountId": 1,
    "Settings":{
        "Delay":"1000"
    }
}
```

The possible command "Kind" values are Stop, Pause, Resume and Configure.  The Settings dictionary will only be needed if the Kind  value is set to "Configure"

When quoting a price for the above work, please make provision for a quick kick-off meeting, via Zoom, to make sure we're both on the same page.

Looking forward to hearing from you...
