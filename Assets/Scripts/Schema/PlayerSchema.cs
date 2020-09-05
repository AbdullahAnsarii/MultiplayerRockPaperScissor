// 
// THIS FILE HAS BEEN GENERATED AUTOMATICALLY
// DO NOT CHANGE IT MANUALLY UNLESS YOU KNOW WHAT YOU'RE DOING
// 
// GENERATED USING @colyseus/schema 0.5.17
// 

using Colyseus.Schema;

public class PlayerSchema : Schema {
	[Type(0, "string")]
	public string SessionID = "";

	[Type(1, "number")]
	public float Choice = 0;

	[Type(2, "ref", typeof(USER))]
	public USER User = new USER();
}

