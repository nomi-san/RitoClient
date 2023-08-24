R"(

var DataStore = new function () {
    native function LoadDataStore();
    native function SaveDataStore();

    let _data;
    function data() {
        if (_data) return _data;
        try {
            var object = JSON.parse(LoadDataStore());
            return _data = new Map(Object.entries(object));
        } catch {
            return _data = new Map();
        }
    }

    function commit() {
        var object = Object.fromEntries(data());
        SaveDataStore(JSON.stringify(object));
    }

    return {
        [Symbol.toStringTag]: 'DataStore',
        has(key) {
            return data().has(key);
        },
        get(key, fallback = undefined) {
            if (!data().has(key)) {
                return fallback;
            }
            return data().get(key);
        },
        set(key, value) {
            data().set(key, value);
            commit();
        },
        remove(key) {
            var result = data().delete(key);
            commit();
            return result;
        }
    };
};

)"