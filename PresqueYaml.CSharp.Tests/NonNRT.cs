namespace  MagnusOpera.PresqueYaml.Tests.CSharp;

#nullable disable

public class PersonNonNRT {
    public PersonNonNRT(string firstname, string lastname) {
        FirstName = firstname;
        LastName = lastname;
    }

    public string FirstName { get; init; }
    public string LastName { get; init; }
}

public class TestCSharpNonNRT
{
    [Test]
    public void CheckNonNRT() {
        var yaml = "firstName: John";
        var node = YamlParser.Read(yaml);
        var person = YamlSerializer.Deserialize<PersonNonNRT>(node);
        Assert.That(person.FirstName, Is.EqualTo("John"));
        Assert.That(person.LastName, Is.Null);
    }

    [Test]
    public void CheckNonNRTRequired() {
        var yaml = "lastName: Doe";
        var node = YamlParser.Read(yaml);
        var person = YamlSerializer.Deserialize<PersonNonNRT>(node);
        Assert.That(person.LastName, Is.EqualTo("Doe"));
        Assert.That(person.FirstName, Is.Null);
    }
}
