## Return Types

An async method can have the following return types:

- [Task](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task)
- [Task\<TResult\>](https://docs.microsoft.com/en-us/dotnet/api/system.threading.tasks.task-1)
- [void](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/builtin-types/void). `async void` methods are generally discouraged for code other than event handlers because callers cannot `await` those methods and must implement a different mechanism to report successful completion or error conditions.
- Starting with C# 7.0, any type that has an accessible `GetAwaiter` method. The `System.Threading.Tasks.ValueTask<TResult>` type is one such implementation. It is available by adding the NuGet package `System.Threading.Tasks.Extensions`.

The async method can't declare any [in](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/in-parameter-modifier), [ref](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/ref) or [out](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/out-parameter-modifier) parameters, nor can it have a [reference return value](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/classes-and-structs/ref-returns), but it can call methods that have such parameters.

You specify `Task<TResult>` as the return type of an async method if the [return](https://docs.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/return) statement of the method specifies an operand of type `TResult`. You use `Task` if no meaningful value is returned when the method is completed. That is, a call to the method returns a `Task`, but when the `Task` is completed, any `await` expression that's awaiting the `Task` evaluates to `void`.

You use the `void` return type primarily to define event handlers, which require that return type. The caller of a `void`-returning async method can't await it and can't catch exceptions that the method throws.

Starting with C# 7.0, you return another type, typically a value type, that has a `GetAwaiter` method to minimize memory allocations in performance-critical sections of code.

For more information and examples, see [Async Return Types](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/concepts/async/async-return-types).