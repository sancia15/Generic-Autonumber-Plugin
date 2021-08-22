using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xrm.Sdk;
using Microsoft.Xrm.Sdk.Query;
using CFS.GenericAutonumberSeries.Main.Variables;

namespace CFS.GenericAutonumberSeries.Main
{

    public class AutonumberPlugin:IPlugin
    {
        //declare 
      
        string prefix = string.Empty;
        string seperator = string.Empty;
        string count = string.Empty;

        public void Execute(IServiceProvider serviceProvider)
        {
            try
            {

                IPluginExecutionContext context = (IPluginExecutionContext)serviceProvider.GetService(typeof(IPluginExecutionContext));
                //obtain the tracing service: for logs
                ITracingService trace = (ITracingService)serviceProvider.GetService(typeof(ITracingService));
                //Obtain the execution context from the service provider.
                IOrganizationServiceFactory serviceFactory = (IOrganizationServiceFactory)serviceProvider.GetService(typeof(IOrganizationServiceFactory));
                //For the current user who has logged in
                IOrganizationService service = serviceFactory.CreateOrganizationService(context.UserId);
                //For create
                if (context.InputParameters.Contains("Target") && context.InputParameters["Target"] is Entity && context.MessageName.ToLower() == Constants.Operations.create)
                {
                    Entity targetEntity = context.InputParameters["Target"] as Entity;
                    if (targetEntity.LogicalName == Constants.Entities.account)
                    {
                        try
                        {
                            StringBuilder autoNumber = new StringBuilder();
                            QueryExpression qeAutoNumberConfig = QueryAutonumberConfiguration();
                            EntityCollection ecAutoNumberConfig = service.RetrieveMultiple(qeAutoNumberConfig);

                            if (ecAutoNumberConfig.Entities.Count == 0)
                            {
                                return;
                            }
                            Entity updateAutoNumberConfig = new Entity(Constants.Entities.cfs_autonumberingconfiguration);
                            AutonumberCalulate(service, targetEntity, ecAutoNumberConfig, updateAutoNumberConfig, autoNumber);

                        }
                        catch (Exception ex)
                        {
                            throw new InvalidPluginExecutionException(ex.Message);
                        }
                    }

                }
            }
            catch (Exception ex)
            {
                throw new InvalidPluginExecutionException(ex.Message);
            }
}
        

        //For autonumber count increment
        private void AutonumberCalulate(IOrganizationService service,Entity targetEntity,EntityCollection ecAutoNumberingConfig, Entity updateAutoNumberingConfig, StringBuilder autonumber)
        {
            foreach (Entity entity in ecAutoNumberingConfig.Entities)
            {
                if (entity.Attributes[Constants.AutonumberConfigurationEntity.cfs_entityname].ToString().ToLower() == targetEntity.LogicalName.ToString().ToLower())
                {
                    if (entity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_prefix))
                    {
                        prefix = entity.GetAttributeValue<string>(Constants.AutonumberConfigurationEntity.cfs_prefix);

                    }
                    if (entity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_separator))
                    {
                        seperator = entity.GetAttributeValue<string>(Constants.AutonumberConfigurationEntity.cfs_separator);
                    }
                    if (entity.Attributes.Contains(Constants.AutonumberConfigurationEntity.cfs_count))
                    {
                        count = entity.GetAttributeValue<string>(Constants.AutonumberConfigurationEntity.cfs_count);
                    }


                    int tempCount = int.Parse(count);
                    tempCount++;
                    count = tempCount.ToString("0000");
                    updateAutoNumberingConfig.Id = entity.Id;
                    updateAutoNumberingConfig[Constants.AutonumberConfigurationEntity.cfs_count] = count;
                    service.Update(updateAutoNumberingConfig);
                    autonumber.Append(prefix + seperator + count);
                    break;

                }
            }
            targetEntity[Constants.AutonumberFields.cfs_opportunitynumber] = autonumber.ToString();
            //targetEntity[Constants.AutonumberFields.cfs_opportunitynumber] = autonumber.ToString();
            //targetEntity[Constants.AutonumberFields.cfs_contactnumber] = autonumber.ToString();
            //service.Update(targetEntity);


        }

        //For entites and column set using QueryExpression
        private static QueryExpression QueryAutonumberConfiguration()
        {

            return new QueryExpression(Variables.Constants.AutonumberConfigurationEntity.cfs_autonumberfield)
            {
                EntityName = Constants.Entities.cfs_autonumberingconfiguration,
                ColumnSet = new ColumnSet(Constants.AutonumberConfigurationEntity.cfs_prefix, Constants.AutonumberConfigurationEntity.cfs_count, Constants.AutonumberConfigurationEntity.cfs_autonumberfield, Constants.AutonumberConfigurationEntity.cfs_incrementby, Constants.AutonumberConfigurationEntity.cfs_separator, Constants.AutonumberConfigurationEntity.cfs_entityname)

            };
        }



    }
}
