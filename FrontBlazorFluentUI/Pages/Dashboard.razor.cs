

namespace Laboratory.Front.Pages;

public partial class Dashboard
{
    protected override void OnInitialized()
    {
        LayoutService.Title = "Dashboard";

        var data = new TestTable("Persons", "PersonId", "Name", "BirthDate", "Description")
            .AddRow("A 10895", "F Jean Martin", "4 1985-03-16", "پ During the deserialization process, the state of an object will be reconstructed from the serialized data stream which can contain dangerous operations.")
            .AddRow("B 10944", "C António Langa", "5 1991-12-01", "آ During the deserialization process, the state of an object will be reconstructed from the serialized data stream which can contain dangerous operations.")
            .AddRow("C 11203", "B Julie Smith", "3 1958-10-10", "ب For example, a well-known attack vector consists in serializing an object of type TempFileCollection with arbitrary files (defined by an attacker) which will be deleted on the application deserializing this object (when the finalize() method of the TempFileCollection object is called). This kind of types are called \"gadgets\".")
            .AddRow("D 11205", "A Nur Sari", "2 1922-04-27", "ج myBinaryFormatter.Deserialize(stream); // Noncompliant: a binder is not used to limit types during deserialization")
            .AddRow("E 11898", "D Jose Hernandez", "6 2011-05-03", "")
            .AddRow("F 12130", "E Kenji Sato", "1 2004-01-09", "ج JavaScriptSerializer should use a resolver implementing a whitelist to limit types during deserialization (at least one exception should be thrown or a null value returned):");

        TestTableData = new TestTableViewDataProvider(data);
    }

    public ITableViewDataProvider TestTableData { get; set; }

}
