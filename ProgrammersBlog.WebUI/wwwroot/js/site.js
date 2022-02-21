function convertFirstLetterToUpperCase(text) {
    console.log(text);
    if (text === 'true') {
        console.log('deneme');
        return 'Evet'
    }
    return 'Hayır'
    //return text.charAt(0).toUpperCase() + text.slice(1);

}
function convertToShortDate(dateString) {
    const shortDate = new Date(dateString).toLocaleDateString('en-US');
    return shortDate;
}

function getJsonNetObject(obj, parentObj) {
    // check if obj has $id key.
    var objId = obj["$id"];
    if (typeof (objId) !== "undefined" && objId != null) {
        // $id key exists, so you have the actual object... return it
        return obj;
    }
    // $id did not exist, so check if $ref key exists.
    objId = obj["$ref"];
    if (typeof (objId) !== "undefined" && objId != null) {
        // $ref exists, we need to get the actual object by searching the parent object for $id
        return getJsonNetObjectById(parentObj, objId);
    }
    // $id and $ref did not exist... return null
    return null;
}

// function to return a JSON object by $id
// parentObj: the top level object containing all child objects as serialized by JSON.NET.
// id: the $id value of interest
function getJsonNetObjectById(parentObj, id) {
    // check if $id key exists.
    var objId = parentObj["$id"];
    if (typeof (objId) !== "undefined" && objId != null && objId == id) {
        // $id key exists, and the id matches the id of interest, so you have the object... return it
        return parentObj;
    }
    for (var i in parentObj) {
        if (typeof (parentObj[i]) == "object" && parentObj[i] != null) {
            //going one step down in the object tree
            var result = getJsonNetObjectById(parentObj[i], id);
            if (result != null) {
                // return found object
                return result;
            }
        }
    }
    return null;
}




(function () {

    function type(value) {

        var t = typeof (value);

        if (t == "object" && value instanceof Array) {
            return "array";
        }

        if (t == "object" && value && "$id" in value && "$values" in value) {
            return "array";
        }

        return t;
    }

    function TypeConverterFactory() {

        var converters = {};

        var defaultConverter = {
            fromJson: function (value) { return value; },
            toJson: function (value) { return value; },
        };

        this.create = function (type) {
            var converter = converters[type];
            if (!converter) return defaultConverter;
            return converter;
        };

        this.register = function (type, converter) {
            converters[type] = converter;
            converter.valueConverter = this.valueConverter;
        };
    }



    function ObjectConverter() {

        this.fromJson = function (obj) {

            if (obj == null) return null;

            if ("$ref" in obj) {
                var reference = this.dictionary[obj.$ref];
                return reference;
            }

            if ("$id" in obj) {
                this.dictionary[obj.$id] = obj;
                delete obj.$id;
            }

            for (var prop in obj) {
                obj[prop] = this.valueConverter.convertFromJson(obj[prop]);
            }

            return obj;

        }

        this.toJson = function (obj) {

            var id = 0;

            if (~(id = this.dictionary.indexOf(obj))) {
                return { "$ref": (id + 1).toString() };
            }

            var convertedObj = { "$id": this.dictionary.push(obj).toString() };

            for (var prop in obj) {
                convertedObj[prop] = this.valueConverter.convertToJson(obj[prop]);
            }

            return convertedObj;

        }

    }

    function ArrayConverter() {

        var self = this;

        this.fromJson = function (arr) {

            if (arr == null) return null;

            if ("$id" in arr) {

                var values = arr.$values.map(function (item) {
                    return self.valueConverter.convertFromJson(item);
                });

                this.dictionary[arr.$id] = values;

                delete arr.$id;

                return values;
            }

            return arr;

        }

        this.toJson = function (arr) {

            var id = 0;

            if (~(id = this.dictionary.indexOf(arr))) {
                return { "$ref": (id + 1).toString() };
            }

            var convertedObj = { "$id": this.dictionary.push(arr).toString() };

            convertedObj.$values = arr.map(function (arrItem) {
                return self.valueConverter.convertToJson(arrItem);
            });

            return convertedObj;

        }

    }

    function ValueConverter() {

        this.typeConverterFactory = new TypeConverterFactory();
        this.typeConverterFactory.valueConverter = this;
        this.typeConverterFactory.register("array", new ArrayConverter);
        this.typeConverterFactory.register("object", new ObjectConverter);

        this.dictionary = {};

        this.convertToJson = function (valor) {

            var converter = this.typeConverterFactory.create(type(valor));
            converter.dictionary = this.dictionary;
            return converter.toJson(valor);

        }

        this.convertFromJson = function (valor) {

            var converter = this.typeConverterFactory.create(type(valor));
            converter.dictionary = this.dictionary;
            return converter.fromJson(valor);

        }

    }


    function JsonRecursive() {

        this.valueConverter = new ValueConverter();

    }

    JsonRecursive.prototype.convert = function (obj) {
        this.valueConverter.dictionary = [];
        var converted = this.valueConverter.convertToJson(obj);
        return converted;

    }

    JsonRecursive.prototype.parse = function (string) {
        this.valueConverter.dictionary = {};
        var referenced = JSON.parse(string);
        return this.valueConverter.convertFromJson(referenced);

    }


    JsonRecursive.prototype.stringify = function (obj) {

        var converted = this.convert(obj);
        var params = [].slice.call(arguments, 1);
        return JSON.stringify.apply(JSON, [converted].concat(params));

    }

    if (window) {

        if (window.define) {
            //to AMD (require.js)
            window.define(function () {
                return new JsonRecursive();
            });

        } else {
            //basic exposition
            window.jsonRecursive = new JsonRecursive();
        }

        return;
    }

    if (global) {
        // export to node.js
        module.exports = new JsonRecursive();
    }


}());

function getTodaysDate() {
    let today = new Date();
    return `${today.getDate()}/${today.getMonth()+1}/${today.getDay()}}`
}