namespace  MagnusOpera.PresqueYaml.Tests.CSharp;


public class PersonNRT {
    public PersonNRT(string firstname, string? lastname) {
        FirstName = firstname;
        LastName = lastname;
    }

    public string FirstName { get; init; }
    public string? LastName { get; init; }
}


public class TestCSharpNRT
{
    [Test]
    public void CheckNRT() {
        var yaml = "firstName: John";
        var node = YamlParser.Read(yaml);
        var person = YamlSerializer.Deserialize<PersonNRT>(node);
        Assert.That(person.FirstName, Is.EqualTo("John"));
        Assert.That(person.LastName, Is.Null);
    }

    [Test]
    public void CheckNRTRequired() {
        var yaml = "lastName: Doe";
        var node = YamlParser.Read(yaml);
        Assert.That(() => YamlSerializer.Deserialize<PersonNRT>(node), Throws.Exception);
    }
}
