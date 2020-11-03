using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Activities;
using Microsoft.Xrm.Sdk.Workflow;
using Microsoft.Xrm.Sdk;

namespace CreateEmailFromCutomWorkflow
{
    public sealed partial class Program : CodeActivity
    {
        protected override void Execute(CodeActivityContext executionContext)
        {
            IWorkflowContext context = executionContext.GetExtension<IWorkflowContext>();
            IOrganizationServiceFactory organizationServiceFactory = executionContext.GetExtension<IOrganizationServiceFactory>();
            IOrganizationService service = organizationServiceFactory.CreateOrganizationService(context.UserId);

            Guid caseId = this.inputEntity.Get(executionContext).Id;
            Entity email = new Entity();
            email.LogicalName = "email";
            email["subject"] = "Case record created - "+inputEntityCaseNumber.Get(executionContext);
            email["regardingobjectid"] = new EntityReference("incident", caseId);
            email["cts_source"] = "Created from Custom Workflow";
            Guid emailId = service.Create(email);
            this.emailCreated.Set(executionContext, new EntityReference("email", emailId));
        }
        [RequiredArgument]
        [Input("InputEntity")]
        [ReferenceTarget("incident")]
        public InArgument<EntityReference> inputEntity { get; set; }

        [RequiredArgument]
        [Input("InputEntityCaseNumber")]
        [ReferenceTarget("ticketnumber")]
        public InArgument<String> inputEntityCaseNumber { get; set; }

        [Output ("EmailCreated")]
        [ReferenceTarget("email")]
        public OutArgument<EntityReference> emailCreated { get; set; }
    }
}
