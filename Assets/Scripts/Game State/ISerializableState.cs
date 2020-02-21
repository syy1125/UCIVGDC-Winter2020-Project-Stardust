public interface ISaveLoad<T>
	where T : struct
{
	T Save();
	void Load(T serialized);
}