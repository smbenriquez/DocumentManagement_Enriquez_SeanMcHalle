namespace DocumentManagement_Enriquez_SeanMcHalle.Business.Generics
{
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public T Value { get; }
        public string ErrorMessage { get; }

        private Result(T value, bool isSuccess, string errorMessage)
        {
            Value = value;
            IsSuccess = isSuccess;
            ErrorMessage = errorMessage;
        }

        public static Result<T> Success(T value) => new(value, true, string.Empty);
        public static Result<T> Failure(string errorMessage) => new(default, false, errorMessage);
    }
}
