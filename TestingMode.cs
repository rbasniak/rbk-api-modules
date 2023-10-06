using System;

public static class TestingMode
{
	public static DatabaseType GetMode(string project)
	{
		switch (project)
		{
			case "Demo1": return DatabaseType.Native;
			case "Demo2": return DatabaseType.Native;
			case "Demo3": return DatabaseType.Native;
			case "Demo4": return DatabaseType.Native;
			case "Demo5": return DatabaseType.Native;
			default:
				throw new NotSupportedException();
		}

	}
}

public enum DatabaseType
{
	Native,
	SQLite
}