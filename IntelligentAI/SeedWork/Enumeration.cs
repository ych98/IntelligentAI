using System.Reflection;

namespace IntelligentAI.SeedWork;

public abstract class Enumeration : IComparable
{
    public int Id { get; private set; }
    public string Name { get; private set; }
    public string Description { get; private set; }

    protected Enumeration(int id, string name, string description) => (Id, Name, Description) = (id, name, description);

    public override string ToString() => Name;

    public static IEnumerable<T> GetAll<T>() where T : Enumeration =>
        typeof(T).GetFields(BindingFlags.Public |
                            BindingFlags.Static |
                            BindingFlags.DeclaredOnly)
                    .Select(f => f.GetValue(null))
                    //.Cast<T>();
                    .OfType<T>();

    public override bool Equals(object obj)
    {
        if (obj is not Enumeration otherValue) return false;

        var typeMatches = GetType().Equals(obj.GetType());
        var valueMatches = Id.Equals(otherValue.Id);

        return typeMatches && valueMatches;
    }

    public override int GetHashCode() => Id.GetHashCode();

    public static int AbsoluteDifference(Enumeration firstValue, Enumeration secondValue)
    {
        var absoluteDifference = Math.Abs(firstValue.Id - secondValue.Id);
        return absoluteDifference;
    }

    public static T FromId<T>(int id) where T : Enumeration
    {
        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Id == id);
        if (matchingItem == null)
            throw new InvalidOperationException($"'{id}' 不是 {typeof(T)} 的有效值，请确保 id 参数的有效性");
        return matchingItem;
    }

    public static T FromName<T>(string name) where T : Enumeration
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new InvalidOperationException($"'{name}' 不是 {typeof(T)} 的有效值，请确保 name 参数的有效性");

        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Name == name);

        if (matchingItem == null)
            throw new InvalidOperationException($"'{name}' 不是 {typeof(T)} 的有效值，请确保 name 参数的有效性");

        return matchingItem;
    }

    public static T FromDescription<T>(string description) where T : Enumeration
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new InvalidOperationException($"'{description}' 不是 {typeof(T)} 的有效值，请确保 description 参数的有效性");

        var matchingItem = GetAll<T>().FirstOrDefault(item => item.Description == description);
        if (matchingItem == null)
            throw new InvalidOperationException($"'{description}' 不是 {typeof(T)} 的有效值，请确保 description 参数的有效性");
        return matchingItem;
    }

    public int CompareTo(object other) => Id.CompareTo(((Enumeration)other).Id);
}
