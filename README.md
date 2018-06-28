# RegexContainerUtility
Attribute utility for parsing and storing regular expression results into a user-defined container.

## Usage
The following classes are in the `RegularExpression.Utility` namespace.
### Setting Up The Container
A container can be any `class` with a public parameterless constructor or `struct`. Classes are recommended over structs due to a forced boxing and unboxing step along with a copy that takes place in the `ResultContainer<T>` constructor. Use the `RegexContainerAttribute` to mark a container and give it a regular expression and optional `RegexOptions`. Use `RegexDataAttribute` to specify fields and properties of the container that store data from the regular expression. Example:
```
[RegexContainer(@"(?<X>\d+),(?<Y>\d+),(?<Z>\d+)?")]
struct Point
{
    [RegexData]
    public int X;
    [RegexData]
    public int Y;
    [RegexData]
    public int Z;
}
```
There are three ways to link a field or property with a regular expression group. The default shown above uses the field or property name and a matching explicit group name. An explicit name can also be given to the `RegexDataAttribute` to use instead of the field or property name. Example:
```
[RegexContainer(@"(?<FN>\w+) (?<LastName>\w+)")]
class UserName
{
    [RegexData(MatchID = "FN")]
    public string FirstName { get; set; }
    [RegexData]
    public string LastName { get; set; }
}
```
The third way is using implicit group names with explicit field or property names. **This is not recommended as it requires intimate knowledge of how groups are ordered by the regular expression engine.**
```
[RegexContainer(@"(\d+),(\d+),(\d+)?")]
struct Point
{
    [RegexData(MatchID = "1")]
    public int X;
    [RegexData(MatchID = "2")]
    public int Y;
    [RegexData(MatchID = "3")]
    public int Z;
}
```
Arrays and types that derive from `ICollection<T>` are also supported via the `RegexDataListAttribute`. Currently only delimited lists are supported. Example:
```
//Input: "John Doe, Jane Doe, Bob"
[RegexContainer(@"(?<Users>.+)")]
class Users
{
    [RegexData(MatchID = "Users")]
    [RegexDataList(',')]
    public List<string> UserNames { get; private set; }
}
```
You can turn on or off whitespace trimming via the `TrimWhitespace` optional paramter on the `RegexDataListAttribute`. To use your configured container, use it as the type parameter for the `RegexContainer<T>` class. This class exposes both a `Parse` and `ParseAll` method. The behavior is identical to the `Regex.Match` and `Regex.Matches` behavior, respectively.
```
var container = new RegexContainer<Point>();
ContainerResult<Point> result = container.Parse("1 7 10, 2 5 3"); //result.Value is the point X:1, Y:7, Z:10
ContainerResultCollection<Point> results = container.ParseAll("1 7 10, 2 5 3"); results contains points {1, 7, 10} and {2, 5, 3}
```

## Technical Details
### RegexContainerAttribute
This attribute has one required parameter which is the regular expression that defines capturing groups. An optional parameter `Options` allows you to specify `System.Text.RegularExpressions.RegexOptions` (default: `RegexOptions.None`) just like the `System.Text.RegularExpressions.Regex` class.

The container must have a `public` parameterless constructor. For `class`es, this must be explicit if another constructor is present. Every `struct` has one internally regardless of other constructors present.
### RegexDataAttribute
This attribute has one optional parameter that allows you to specify a group name. Without specifying this group name the name of the property or field is used as the group name.

Any field is a valid field for this attribute. The only restriction on properties is that they must be writeable (define a `set` accessor). Any access modifier is valid as even `private` fields, properties, and property accessors can be written via reflection.
### RegexDataListAttribute
This attribute has one required parameter which specifies the delimiter used to split the group match. An optional parameter `TrimWhitespace` allows you to turn on or off trimming of leading and trailing whitespace (default: `true`). This trimming occurs after the split on the delimiter and before the value is further processed (added to the collection, passed to a subcontainer, etc).
### RegexContainer\<T\>
If a match is not made to the container's regular expression, a `ContainerResult<T>` with the `Success` property set to `false` will be returned and the related field or property will not be set (even to a default). Optional groups will not cause a failed match. When using the `ParseAll` method, the `ContainerResultCollection<T>` will contain only successful `ContainerResult<T>` matches. A `ContainerResult<T>` will also be unsuccessful if a field or property set type mismatch occurs such as attempting to set `abc123` to an `int`. If a field or property is itself a container, a failed non-optional match in the sub-container will cause the root container to be unsuccessful as well so carefully choose which groups are not optional (optional ex. `(?<group>.+)?`, non-optional ex. `(?<group>.+)`).

`RegexContainer<T>` caches the reflected type structure statically. Per `T`, future calls to the instance constructor or methods do not incur the majority of the reflection overhead.
## Debugging
Debugging should be fairly straightforward. The only general error you should see is a `TypeInitialization` exception. This indicates the container is not configured correctly. Either a list type is used that does not implement `ICollection<T>` or its derivatives, or a non-list type that does not have a `public` parameterless constructor is used.

Casting errors such as a type that cannot be converted from `System.String` will be caught and the container's `Success` property will be set to `false`. An internal exception is also caught which indicates an invalid match to a sub-container as explained in the `RegexContainer<T>` section.
