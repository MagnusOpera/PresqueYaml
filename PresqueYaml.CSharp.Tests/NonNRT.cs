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
        Assert.That(() => YamlSerializer.Deserialize<PersonNonNRT>(node), Throws.Exception);
    }
}
