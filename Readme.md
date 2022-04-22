# FluentValidation
[![CI](https://github.com/BarnettMichael/FluentValidator/actions/workflows/ci.yml/badge.svg)](https://github.com/BarnettMichael/FluentValidator/actions/workflows/ci.yml)

[FluentValidation](#fluentvalidation)
- [FluentValidation](#fluentvalidation)
  - [Validation](#validation)
  - [Parsing](#parsing)
  - [Documentation:](#documentation)
    - [Check](#check)
  - [TODO:](#todo)
    - [Ideas for other methods](#ideas-for-other-methods)

A library that allows you to use a declarative and fluent syntax when validating or parsing an object.

The library can be used in two flavours; validation, and parsing. This is determined by the class that the fluent syntax is initially created from. The `Check` class provides methods that allow the user to fluently perform validation checks against an object. Returning a `Result` object that contains a boolean flag whether or not the validation checks were passed, and in the case where the validation failed a user configurable error message specifying the issue.

## Validation
Sometimes you will want to take an object, run a series of tests against it and determine if some/all/any of the tests resulted in a failure.

## Parsing
When you need to transform an object into a new type only if certain conditions are met, for example when receiving a representation of the data stored in a database and needing to convert that into a domain model which is guaranteed to fulfil certain criteria.

## Documentation:

### Check
The `Check` class provides an entry point to the declaration of a validation pipeline. The pipeline will be executed synchronously and in series, the first instance of a validation 'failure' in the pipeline will result in no further checks being run.
The class exposes a single method `That<TEntity>(TEntity value)` which will return an object that can then be the starting point for a declarative validation pipeline.

```csharp
int ten = 10;
Check.That(10) // This returns the object that can be used to build a validation chain.
```

---

## TODO:
- Move Check and Parse into separate projects/namespaces so can only import one or the other if wanted.
- Full Readme with examples and explanations
- Duplicate Check functionality into Parse once finalised.
- Add ability to pass multiple types of object to the predicate function.
- Add CheckAsync and CheckParallelAsync?

### Ideas for other methods
- Check.That.Or (check that this passes even if previous fail and if so return a true)
- Check.That.Any (check that at least one of the checks pass)
- Result.OrAny (Pass array of checks that will all be checked and if one is true return true even if previous validation was false)
- CheckParallel.That.Is (Check every thing in the chain either in parallel, or at least combine results so know all the failures).
