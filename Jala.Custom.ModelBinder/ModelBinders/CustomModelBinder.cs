using Jala.Custom.ModelBinder.Controllers;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Newtonsoft.Json;

namespace Jala.Custom.ModelBinder.ModelBinders;

public class CustomModelBinder : IModelBinder
{
    public async Task BindModelAsync(ModelBindingContext bindingContext)
    {

        int queryId = 0;

        if (bindingContext.ActionContext.HttpContext.Request.Query.Count > 0)
        {
            var id = bindingContext.ActionContext.HttpContext.Request.Query["id"];
            queryId = int.Parse(id);
        }

        string valueFromBody;
        
        using (var streamReader = new StreamReader(bindingContext.HttpContext.Request.Body))
        {
            valueFromBody = await streamReader.ReadToEndAsync();
        }
        
        if (string.IsNullOrWhiteSpace(valueFromBody) && string.IsNullOrEmpty(valueFromBody))
            return;

        Page? modelInstance = null;
        try
        {
            //Try to deserialize the string to the Page type
            modelInstance = JsonConvert.DeserializeObject<Page>(valueFromBody);
            modelInstance.Id = queryId;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            bindingContext.Result = ModelBindingResult.Failed();
            return;
        }
        
        bindingContext.Result = ModelBindingResult.Success(modelInstance);
    }
}