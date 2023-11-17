namespace  MagnusOpera.PresqueYaml.Tests.CSharp;

public class Person {
    public Person(string firstname, string? lastname) {
        FirstName = firstname;
        LastName = lastname;
    }

    public string FirstName { get; init; }
    public string? LastName { get; init; }
}
