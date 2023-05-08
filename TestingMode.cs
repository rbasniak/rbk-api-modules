using System;

public static class TestingMode
{
	public static DatabaseType GetMode(string project)
	{
		switch (project)
		{
			case "Demo1": return DatabaseType.Native;
			case "Demo2": return DatabaseType.SQLite;
			case "Demo3": return DatabaseType.SQLite;
			case "Demo4": return DatabaseType.SQLite;
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