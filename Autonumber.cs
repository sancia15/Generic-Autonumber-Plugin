using CFS.GenericAutonumberSeries.Main.Variables;
using Microsoft.Xrm.Sdk;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk.Query;
namespace CFS.GenericAutonumberSeries.Main
{
    public class Autonumber : IPlugin

    {
        public void Execute(IServiceProvider serviceProvider)
        {

            IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
            //obtain the tracing service: for logs
            ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
            //Obtain the execution context from the service provider.
            IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
            //For the current user who has logged in
            IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
            try
            {
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity && context.MessageName.ToLower() == Constants.Operations.create)
                {
                    Entity targetEntity = context.InputParameters["Target"] as Entity;
                    EntityCollection autonumberConfigurationCollection = AutonumberEntityCollection(service, targetEntity);
                    if (autonumberConfigurationCollection.Entities.Count == 1)
                    {
                        Entity autonumberentity = autonumberConfigurationCollection.Entities[0];
                        AutoCalculate(service, targetEntity, autonumberentity);
                        //if (autonumberentity.Attributes[Constants.AutonumberConfigurationEntity.cfs_entityname].ToString().ToLower() == targetEntity.LogicalName.ToString().ToLower())
                        //{

                        //}
                    }
                }

            }
            catch (Exception ex)
            {

                 Errorhandling.LogError(ex.StackTrace, ex.Message, "CFS.GenericAutonumberSeries.Main.Accounts","Execute", string.Empty,service);
            }
        }

        private static EntityCollection AutonumberEntityCollection(IOrganizationService service, Entity targetEntity)
        {
            string fetchXMLquery = @"<fetch version='1.0' output-format='xml-platform' mapping='logical' distinct='false'>
                                              <entity name='cfs_autonumberingconfiguration'>
                                                <all-attributes/>
                                                <order attribute='cfs_name' descending='false' />
                                                <filter type='and'>
                                                  <condition attribute='cfs_entityname' operator='eq' value='" + targetEntity.LogicalName.ToLower() + @"' />
                                                </filter>
                                              </entity>
                                            </fetch>";
            EntityCollection autonumberConfigurationCollection = service.RetrieveMultiple(new FetchExpression(fetchXMLquery));
            return autonumberConfigurationCollection;
        }

        private void AutoCalculate(IOrganizationService service,Entity targetEntity, Entity autonumberentity)
            {
                
                        string prefix = string.Empty;
                        string seperator = string.Empty;
                        int count = 0;
                        string fieldname = string.Empty;
                        int incrementby = 0;
                        StringBuilder autoNumber = new StringBuilder();

                        if (autonumberentity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_prefix))
                        {
                            prefix = autonumberentity.GetAttributeValue<string>(Constants.AutonumberConfigurationEntity.cfs_prefix);

                        }
                        if (autonumberentity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_separator))
                        {
                            seperator = autonumberentity.GetAttributeValue<string>(Constants.AutonumberConfigurationEntity.cfs_separator);
                        }
                        if (autonumberentity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_count))
                        {
                            count = autonumberentity.GetAttributeValue<int>(Constants.AutonumberConfigurationEntity.cfs_count);
                        }
                        if (autonumberentity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_autonumberfield))
                        {
                            fieldname = autonumberentity.GetAttributeValue<string>(Constants.AutonumberConfigurationEntity.cfs_autonumberfield);
                        }
                        if (autonumberentity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_incrementby))
                        {
                            incrementby = autonumberentity.GetAttributeValue<int>(Constants.AutonumberConfigurationEntity.cfs_incrementby);
                        }

                        
                         count += incrementby;
                        string currentCounter = count.ToString("0000");
                        Entity updateAutoNumberConfig = new Entity(Constants.Entities.cfs_autonumberingconfiguration);
                        updateAutoNumberConfig.Id = autonumberentity.Id;
                        updateAutoNumberConfig[Constants.AutonumberConfigurationEntity.cfs_count] = count;
                        service.Update(updateAutoNumberConfig);
                        autoNumber.Append(prefix + seperator + currentCounter);
                        Entity updateTargetEntity = new Entity(targetEntity.LogicalName);
                        updateTargetEntity.Id = targetEntity.Id;
                        updateTargetEntity[fieldname] = autoNumber.ToString();
                        //
                        service.Update(updateTargetEntity);
            }
                
            
    }
}

