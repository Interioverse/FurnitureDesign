using System;
using System.Collections;
using System.Collections.Generic;

[Serializable]
public class BoxFilesManager
{
    public string type;
    public string id;
    public string sequence_id;
    public string etag;
    public string name;
    public string created_at;
    public string modified_at;
    public string description;
    public int size;
    public PathCollection path_collection;
    public CreatedBy created_by;
    public ModifiedBy modified_by;
    public string trashed_at;
    public string purged_at;
    public string content_created_at;
    public string content_modified_at;
    public OwnedBy owned_by;
    public SharedLink shared_link;
    public string folder_upload_email;
    //public string parent;
    public string item_status;
    public ItemCollection item_collection;

}

[Serializable]
public class SharedLink
{
    public string url;
    public string download_url;
    public string vanity_url;
    public string vanity_name;
    public string effective_access;
    public string effective_permission;
    public bool is_password_enabled;
    public object unshared_at;
    public int download_count;
    public int preview_count;
    public string access;
    public Permissions permissions;
}

[Serializable]
public class Permissions
{
    public bool can_preview;
    public bool can_download;
    public bool can_edit;
}


[Serializable]
public class CreatedBy
{
    public string type;
    public string id;
    public string name;
    public string login;
}

[Serializable]
public class Entry
{
    public string type;
    public string id;
    public string sequence_id;
    public string etag;
    public string name;
    public FileVersion file_version;
}

[Serializable]
public class FileVersion
{
    public string type;
    public string id;
    public string sha1;
}

[Serializable]
public class ItemCollection
{
    public int total_count;
    public List<Entry> entries;
    public int offset;
    public int limit;
    public List<Order> order;
}

[Serializable]
public class ModifiedBy
{
    public string type;
    public string id;
    public string name;
    public string login;
}

[Serializable]
public class Order
{
    public string by;
    public string direction;
}

[Serializable]
public class OwnedBy
{
    public string type;
    public string id;
    public string name;
    public string login;
}

[Serializable]
public class PathCollection
{
    public int total_count;
    public List<Entry> entries;
}
