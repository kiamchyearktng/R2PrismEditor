namespace lib.remnant2.saves;

public class Options
{
    // By default FowVisitedCoordinates are not parsed and is read as a byte array
    // because we usually not interested in this data and it creates a lot of small objects
    // Set this to true to parse it to the object model
    public bool ParseFowVisitedCoordinates;
}
