public class User
{
    public string DeviceID;
    public string Gender;
    public string Name;
    public string Star;
    public int Age;

    public User(string deviceID, string gender, string name, string star, int age)
    {
        DeviceID = deviceID;
        Gender = gender;
        Name = name;
        Star = star;
        Age = age;
    }

    public User()
    {

    }
}