namespace PresqueYaml.Mapper
open PresqueYaml.Model
open System

type FSharpOptionConverter<'T when 'T : null>(options:YamlSerializerOptions) =
    inherit YamlConverter<'T option>()

    override _.Read(node:YamlNode, typeToConvert:Type) =
        let data = YamlSerializer.Deserialize(node, typeof<'T>, options) :?> 'T
        Option.ofObj data
