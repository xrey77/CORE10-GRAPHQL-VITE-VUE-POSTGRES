namespace core10_graphql_postgres.Helpers;

public class GraphQLErrorFilter : IErrorFilter
{
    public IError OnError(IError error)
    {
        // If the error contains your custom exception, use its message
        if (error.Exception is AppException appEx)
        {
            return error.WithMessage(appEx.Message).WithCode("APP_ERROR");
        }
        return error;
    }
}
