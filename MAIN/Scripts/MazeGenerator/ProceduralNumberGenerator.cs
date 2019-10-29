public class ProceduralNumberGenerator {
	public static int currentPosition = 0;
	public const string key = "12341234123412341234";

	public static int GetNextNumber(string seed = key) {
		string currentNum = seed.Substring(currentPosition++ % seed.Length, 1);
		return int.Parse (currentNum);
	}
}
