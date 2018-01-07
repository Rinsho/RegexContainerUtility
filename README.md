# README STILL UNDER CONSTRUCTION
# RegexAttributeUtility
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

