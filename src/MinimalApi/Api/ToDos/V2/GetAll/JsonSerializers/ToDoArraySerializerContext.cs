//using System.Text.Json.Serialization;

//namespace MinimalApi.Api.ToDos.V2.GetAll
//{
//    internal static class AddToDoArraySerializerContextExtension
//    {
//        internal static void AddToDoArraySerializerContext(this IServiceCollection services)
//        {
//            services.ConfigureHttpJsonOptions(options => options.SerializerOptions.TypeInfoResolverChain.Add(ToDoArraySerializerContextV2.Default));
//        }
//    }

//    [JsonSerializable(typeof(Todo[]))]
//    internal sealed partial class ToDoArraySerializerContextV2
//    {
//    }
//}
