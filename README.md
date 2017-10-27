# PandAPI
---- operations

AddObject(payload:any, principalObjectId?, groupId[], userId)


- AddGroup(groupName) 

- AddUser(userName)

GetObjectsForGroup (groupId)

GetObjectsForUser (userId) {
	GetGroupsForUser(userId).Select(g => GetObjectsForGroup(g)).UnionAll();
}

GetGroupsForUser (userId)

- GetUsersForGroup (groupId)




---- data model
User {
	userId:
	groups: [groupIds] // computed.
}

Group {
	groupId
	isPrivate: // true by default
	users: [] // computed. Perhaps store in a separate table if it's going to be big. 
}

UserMembership { // source of the truth
	UserId: // indexed
	GroupId:  // indexed
	
}

StorageObjectGroupMembership { // source of the truth
	ObjectId
	GroupId
}

StorageObject {
	objectId
	parentObjectId?: //only if this object is owned by some other object. Recursive. Immutable.
	groupIds: [] // every object is owned by at least a group.
	
}

