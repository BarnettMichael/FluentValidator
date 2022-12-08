# FluentValidation
[![CI](https://github.com/BarnettMichael/FluentValidator/actions/workflows/ci.yml/badge.svg)](https://github.com/BarnettMichael/FluentValidator/actions/workflows/ci.yml)

- [FluentValidation](#fluentvalidation)
  - [Validation](#validation)
  - [Parsing](#parsing)
  - [Documentation:](#documentation)
    - [Check](#check)
    - [Seed](#seed)
      - [**`Is<TEntity>`**](#istentity)
      - [**Overloads**](#overloads)
      - [**`IsAll<TEntity>`**](#isalltentity)
      - [**Overloads**](#overloads-1)
    - [Result](#result)
      - [**`And<TEntity>`**](#andtentity)
      - [**Overloads**](#overloads-2)
      - [**`AndAll<TEntity>`**](#andalltentity)
      - [**Overloads**](#overloads-3)
- [TODO -](#todo--)
  - [**Structure of documentation**](#structure-of-documentation)
    - [Class](#class)
      - [**`Method1`**](#method1)
      - [**Overloads**](#overloads-4)
      - [**`ExtensionMethod1`**](#extensionmethod1)
      - [**Overloads**](#overloads-5)
  - [TODO:](#todo)
    - [Ideas for other methods](#ideas-for-other-methods)

FluentValidation is a library that allows you to use a declarative and fluent syntax when validating or parsing objects in your codebase. The two branches follow very similar usage patterns but are instantiated from a different root class.

## Validation
`FluentValidator` can be used to validate properties of an object by using the `Check` class.
The `Check` class acts as an entry point to methods that allow the user to fluently declare validation checks to be performed against an object. Returning a `Result` object that contains an `IsSuccess` boolean flag indicating whether or not all the validation checks succeeded. In the case where any validation failed a user configurable error object is also returned in the `Error` property of the `Result`.

## Parsing
You would use the parsing functionality of `FluentValidator` in situations where you need to transform an object into a new type, if and only if certain conditions are met, for example when receiving a representation of a model as json and needing to convert that into a domain model which is guaranteed to fulfil certain criteria in order to be valid.

The `Parse` class acts as an entry point to methods that will attempt to mutate an object into another type, via a series of user defined steps. The end result will be a `ParseResult` class which contains an `IsParsedSuccessfully` boolean flag that indicates whether or not the object was able to be successfully parsed throughout the entire chain. If the object is parsed successfully then it will be available via the `Value` property

## Documentation:

### Check
The `Check` class provides an entry point to the declaration of a validation pipeline. The pipeline will be executed synchronously and in series, i.e the first instance of a validation 'failure' in the pipeline will result in no further checks being run.
The `Check` class has a single method `That<TEntity>(TEntity value)` which will return an object of type `Seed` which is the starting point for a declarative validation pipeline.

**Example**
```csharp
int ten = 10;
var validationSeed = Check.That(10) // This returns the seed object that is then used to build a validation chain where the integer with value 10 will be the input to each of the checks.
```

### Seed
The `Seed<TEntity>` class is the object against which a validation pipeline is actually called. At its core, a `Seed` is simply a wrapper for a `Value` property of type `TEntity`. The `Seed` class makes several extension methods available that add the ability to begin running validation against the `Value` of the `Seed`.

**Extension Methods**
#### **`Is<TEntity>`**

For any `Seed` there will be a `Value` property of type `TEntity`. The `Is` method takes a function called predicate as a parameter. This function must take a single parameter of type `TEntity` and return a bool. This 'predicate' function will then be called with the `Value` property of the `Seed` as its input. The second parameter of the `Is` method is a string that will be returned as part of the result in the case when the `Value` fails the validation check.

**Example**
```csharp
// Set up the validation of a specified integer to check if the integer is greater than zero.

// Declare a validation function of type Func<TEntity,bool> that will be called to check the integer.
Func<int, bool> greaterThanZero = i => i > 0;

// Declare a chain of rules that you want to run against the integer.
// with an explicit string to be returned in the case of a validation failure.
Result<int> result = Check.That(50).Is(greaterThanZero, error: "The value was not greater than zero");

// The result of the chain of validation will then be stored in the result variable. Indicating that 50 passes the greaterThanZero check.
```
#### **Overloads**
| Signature | Returns |
| --- | --- |
| Is(`Func<TEntity, bool>` predicate, `string` error) | `Result<TEntity>` |
| Is(`Func<TEntity, bool>` predicate, `TError` error) | `Result<TEntity, TError>` |
| Is(`Func<TEntity, bool>` predicate, `Func<TEntity, TError>` errorGenerator) | `Result<TEntity, TError>` |

There are two overloads of the `Is` method that provide enhanced error handling capabilities. In the case when you want to use something other than a hard coded string you can either pass in a specific object to represent the error, or a function that takes the `Value` in as a parameter and returns an object to represent an error in that case.

**Examples**
```csharp
// The value that will be checked
var ten = 10;
// Function to check if the value is an odd integer.
Func<int, bool> anOddInteger = i => i % 2 == 1;

// Using an Enum as an example of a type that could be used to track what sort of validation has failed.
enum ErrorType { NoError, NotAnOddInteger, NotANegativeNumber }

// In order to have an enum as the error stored within the Result object the Is method should be called with the desired enum value as the second parameter.
Result<int, ErrorType> result = Check.That(ten).Is(anOddInteger, error: NotAnOddInteger);

// Or you can use a function to generate an error type TError based on the value
// TError here is set to string
Func<int, string> generateErrorString = i => $"Value: {i} is not an odd number";

// In order to return an error that is dependent on the value being passed, an error generator function can be passed as the second parameter.
Result<int, string> result = Check.That(ten).Is(anOddInteger, generateErrorString);
```

#### **`IsAll<TEntity>`**
For any `Seed` there will be a `Value` property of type `TEntity`. The `IsAll` method takes an array of functions called predicate as a parameter. Each function must take a single parameter of type `TEntity` and return a boolean. These 'predicate' functions will then be called with the `Value` property of the `Seed` as their input. There are overloads of this method that also allow customisation of the errors returned on validation failure.

The value of the `IsAll` method is that it allows for the construction of a validation pipeline at runtime by passing an array of checks, the contents of which could be determined anyway that is desired.

**Example**
```csharp
// Set up the validation of a specified integer to check if the integer is greater than zero.

// Declare validation functions of type Func<TEntity,bool> that will be called to check the integer.
Func<int, bool> greaterThanZero = i => i > 0;
Func<int, bool> isEven = i => i % 2 == 0;

// Declare a chain of rules that you want to run against the integer.
// with an explicit string to be returned in the case of a validation failure.
Result<int> result = Check.That(50).IsAll(greaterThanZero, isEven);

// The result of the chain of validation will then be stored in the result variable. Indicating that 50 passes the greaterThanZero check and isEven check.
```

#### **Overloads**
| Signature | Returns |
| --- | --- |
| IsAll(params (Func<TEntity, bool> predicate, TError error)[] checks) | Result<TEntity, TError> |
| IsAll(params (Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator)[] checks)  | Result<TEntity, TError> |

**Examples**
```csharp
// The value that will be checked
var ten = 10;
// Functions to check if the value is an odd integer.
Func<int, bool> anOddInteger = i => i % 2 == 1;
Func<int, bool> aNegativeInteger = i => i < 0;
Func<int, bool> isDivisibleBy10 = i +> i % 10 == 0;

// Using an Enum as an example of a type that could be used to track what sort of validation has failed.
enum ErrorType { NoError, NotAnOddInteger, NotANegativeNumber, NotDivisibleBy10 }

// In order to have an enum as the error stored within the Result object the IsAll method should be called with an array of tuples that combine the validation function and the desired failure enum.
Result<int, ErrorType> result = Check.That(ten).IsAll(
  (anOddInteger, ErrorType.NotAnOddInteger),
  (aNegativeInteger, ErrorType.NotANegativeNumber),
  (isDivisibleBy10, ErrorType.NotDivisibleBy10));

// Or you can use functions to generate an error type TError based on the value
// TError here is set to string
Func<int, string> generateIsNotOddErrorString = i => $"Value: {i} is not an odd number";
Func<int, string> generateIsNotANegativeNumberErrorString = i => $"Value: {i} is not a negative number";
Func<int, string> generateIsNotDivisibleBy10ErrorString = i => $"Value: {i} is not divisible by 10";

// In order to return an error that is dependent on the value being passed, an error generator function can be passed as the second part of the tuple.
Result<int, string> result = Check.That(ten).IsAll(
  (anOddInteger, generateIsNotOddErrorString),
  (aNegativeInteger, generateIsNotANegativeNumberErrorString),
  (isDivisibleBy10, generateIsNotDivisibleBy10ErrorString));

```

### Result
The  `Result<TEntity>` and `Result<TEntity, TError>` classes are the type that is returned by every validation method. There are also extension methods of these classes to allow chaining of further validation predicates in a fluent builder pattern. The `Result` classes are wrappers around the `Value` property which was initially passed in to the `Seed<TEntity>`, There is also an `IsSuccess` boolean property that indicates if all of the validation exectuted up to this point was successfull and an `Error` property that will be populated with a value of the correct type if the validation was not successful. This `Error` property will indicate at which point in the validation chain the first validation failure occurred.

**Extension Methods**
#### **`And<TEntity>`**
Allows additional chaining of validation methods onto a prexisting validation result.

**Example**
```CSharp
var num = 1d;
Func<double, bool> isPositive = d => d > 0;
Func<double, bool> IsOdd = d => d % 2 == 1;
Func<double, bool> isZero = d => d == 0;
string isNotOddError = "The value is not odd";
string isNotZeroError = "The value is not zero.";

var result = Check.That(num)
    .Is(isPositive, "The value is not positive.") // Initial check returns a Result<TEntity>
    // Further validation predicates can be added by calling the And extension method. They also return a Result<TEntity>
    .And(IsOdd, isNotOddError) 
    .And(isZero, isNotZeroError);
```
#### **Overloads**
| Signature | Returns |
| --- | --- |
| And<TEntity, TError>(Func<TEntity, bool> predicate, TError error) | Result<TEntity, TError> |
| And<TEntity, TError>(Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator)  | Result<TEntity, TError> |

**Examples**
```CSharp
// Using an Enum as an example of a type that could be used to track what sort of validation has failed.
enum ErrorType { NoError, NotPositive, NotZero }

// The value to be validated.
var numErrorType = 1d;

// Some validation predicates.
Func<double, bool> isPositive = d => d > 0;
Func<double, bool> isZero = d => d == 0;


var result = Check.That(numErrorType)
    // The first check returns a Result<TEntity,TError>
    .Is(isPositive, ErrorType.NotPositive)
    // which can then have further predicates chained like so.
    .And(isZero, ErrorType.NotZero);

// Using functions to generate a TError value. In this example TErrror is string.

// The value to be validated.
var numGenerator = 1d;

// Some validation predicates and their corresponding Error generation functions.
Func<double, bool> isPositive = d => d > 0;
Func<double, string> isNotPositiveError = (d) => $"Validation failed: {d} is not positive.";

Func<double, bool> isZero = d => d == 0;
Func<double, string> isNotZeroError = (d) => $"Validation failed: {d} is not zero.";

var result = Check.That(numGenerator)
    // First check returns a Result<TEntity,TError>
    .Is(isPositive, isNotPositiveError)
    // which can have additional predicates applied as validation like so.
    .And(isZero, isNotZeroError);
```

#### **`AndAll<TEntity>`**
Allows additional chaining of an array of validation methods onto a prexisting validation result. It will return a `Result<TEntity>` when called. The validation will apply each predicate in the order that they are enumerated from the `IEnumerable` stopping after the first failure.

**Example**
```CSharp
// The value to be validated
var num = 100;

// Functions that will be used to validate the value.
Func<int, bool> isPositive = i => i > 0;
Func<int, bool> IsDivisibleByTen = i => i % 10 == 0;
Func<int, bool> isEven = i => i % 2 == 0;
Func<int, bool> isLessThanAThousand = i => i < 1000;

// Calling the validation chain, passing each of the validation functions as parameters to AndAll
var result = Check.That(num)
    .Is(isPositive)
    .AndAll(IsDivisibleByTen, isEven, isLessThanAThousand);

```

#### **Overloads**
| Signature | Returns |
| --- | --- |
| AndAll<TEntity, TError>(params (Func<TEntity, bool> predicate, TError error)[] checks) | Result<TEntity, TError> |
| AndAll<TEntity, TError>(params (Func<TEntity, bool> predicate, Func<TEntity, TError> errorGenerator)[] checks) | Result<TEntity, TError> |


**Examples**
```CSharp
// The value that will be validated 
var num = 1d;

// Functions that will be used to validate the value
Func<double, bool> isPositive = d => d > 0;
Func<double, bool> IsOdd = d => d % 2 == 1;
Func<double, bool> isZero = d => d == 0;

// Using an Enum as an example of a type that could be used to track what sort of validation has failed.
enum ErrorType { NoError, NotPositive, NotZero, NotOdd }

// Calling the validation chain where each validation method also registers the error type to be returned in the case
// that the validation fails.
var resultWithErrorType = Check.That(num)
    .Is(isPositive, ErrorType.NotPositive)
    .AndAll((IsOdd, ErrorType.NotOdd), (isZero, ErrorType.NotZero));


// Functions that take in TEntity and return TEntity, in this case a string is generated.
Func<double, string> isNotPositiveErrorGenerator = d => $"The value:{d} is not positive.";
Func<double, string> isNotOddErrorGenerator = d => $"The value:{d} is not odd";
Func<double, string> isNotZeroErrorGenerator = d => $"The value:{d} is not zero.";

// Calling the validation chain where each validation method also registers the error generator to be called in the case
// that the validation fails.
var resultWithErrorGenerator = Check.That(num)
    .Is(isPositive, isNotPositiveErrorGenerator)
    .AndAll((IsOdd, isNotOddErrorGenerator), (isZero, isNotZeroErrorGenerator)); 

```
---
# TODO -
- Fix comments in example methods for Result and ResultExtensions.
---

## **Structure of documentation**
### Class
**Methods**
#### **`Method1`**
**Example**
#### **Overloads**
**Examples**
**ExtensionMethods**
#### **`ExtensionMethod1`**
**Example**
#### **Overloads**
**Examples**

---
## TODO:
- Move Check and Parse into separate projects/namespaces so can only import one or the other if wanted.
- Full Readme with examples and explanations
- Duplicate Check functionality into Parse once finalised.
- Add ability to pass multiple types of object to the predicate function.
- Add CheckAsync and CheckParallelAsync?
- Rename project
  - something to do with the fact that using functions as parameters?
  - Basically an implementation of the fluentbuilder pattern so maybe something to do with builder?
    - validation builder dotnet?
    - validation bob dotnet
    - fvb dotnet
    - fluent validation builder dotnet
      - fl-ui-do validation?
      - fluint dotnet

### Ideas for other methods
- Check.That.Or (check that this passes even if previous fail and if so return a true)
- Check.That.Any (check that at least one of the checks pass)
- Result.OrAny (Pass array of checks that will all be checked and if one is true return true even if previous validation was false)
- CheckParallel.That.Is (Check every thing in the chain either in parallel, or at least combine results so know all the failures).
- CheckAsync / CheckParallelAsync : Async versions of the check and check parallel classes
