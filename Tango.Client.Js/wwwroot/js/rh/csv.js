/**
 * csv
 *
 * This document is licensed as free software under the terms of the
 * MIT License: http://www.opensource.org/licenses/mit-license.php
 *
 * Acknowledgements:
 * For legal purposes I'll include the "NO WARRANTY EXPRESSED OR IMPLIED.
 * USE AT YOUR OWN RISK.". Which, in 'layman's terms' means, by using this
 * library you are accepting responsibility if it breaks your code.
 *
 * Legal jargon aside, I will do my best to provide a useful and stable core
 * that can effectively be built on.
 *
 * Copyrighted 2012 by Evan Plaice.
 */
var Csv = (function () {
    function Csv() {
    }
    Csv.toArray = function (csv, options, callback) {
        options = (options !== undefined ? options : {});
        var config = {};
        config.callback = ((callback !== undefined && typeof (callback) === 'function') ? callback : false);
        config.separator = 'separator' in options ? options.separator : Csv.defaults.separator;
        config.delimiter = 'delimiter' in options ? options.delimiter : Csv.defaults.delimiter;
        var state = (options.state !== undefined ? options.state : {});
        // setup
        options = {
            delimiter: config.delimiter,
            separator: config.separator,
            onParseEntry: options.onParseEntry,
            onParseValue: options.onParseValue,
            state: state
        };
        var entry = Csv.parsers.parseEntry(csv, options);
        // push the value to a callback if one is defined
        if (!config.callback) {
            return entry;
        }
        else {
            config.callback('', entry);
        }
    };
    Csv.toArrays = function (csv, options, callback) {
        options = (options !== undefined ? options : {});
        var config = {};
        config.callback = ((callback !== undefined && typeof (callback) === 'function') ? callback : false);
        config.separator = 'separator' in options ? options.separator : Csv.defaults.separator;
        config.delimiter = 'delimiter' in options ? options.delimiter : Csv.defaults.delimiter;
        // setup
        var data = [];
        options = {
            delimiter: config.delimiter,
            separator: config.separator,
            onPreParse: options.onPreParse,
            onParseEntry: options.onParseEntry,
            onParseValue: options.onParseValue,
            onPostParse: options.onPostParse,
            start: options.start,
            end: options.end,
            state: {
                rowNum: 1,
                colNum: 1
            }
        };
        // onPreParse hook
        if (options.onPreParse !== undefined) {
            options.onPreParse(csv, options.state);
        }
        // parse the data
        data = Csv.parsers.parse(csv, options);
        // onPostParse hook
        if (options.onPostParse !== undefined) {
            options.onPostParse(data, options.state);
        }
        // push the value to a callback if one is defined
        if (!config.callback) {
            return data;
        }
        else {
            config.callback('', data);
        }
    };
    Csv.toObjects = function (csv, options, callback) {
        options = (options !== undefined ? options : {});
        var config = {};
        config.callback = ((callback !== undefined && typeof (callback) === 'function') ? callback : false);
        config.separator = 'separator' in options ? options.separator : Csv.defaults.separator;
        config.delimiter = 'delimiter' in options ? options.delimiter : Csv.defaults.delimiter;
        config.headers = 'headers' in options ? options.headers : Csv.defaults.headers;
        options.start = 'start' in options ? options.start : 1;
        // account for headers
        if (config.headers) {
            options.start++;
        }
        if (options.end && config.headers) {
            options.end++;
        }
        // setup
        var lines = [];
        var data = [];
        options = {
            delimiter: config.delimiter,
            separator: config.separator,
            onPreParse: options.onPreParse,
            onParseEntry: options.onParseEntry,
            onParseValue: options.onParseValue,
            onPostParse: options.onPostParse,
            start: options.start,
            end: options.end,
            state: {
                rowNum: 1,
                colNum: 1
            },
            match: false,
            transform: options.transform
        };
        // fetch the headers
        var headerOptions = {
            delimiter: config.delimiter,
            separator: config.separator,
            start: 1,
            end: 1,
            state: {
                rowNum: 1,
                colNum: 1
            }
        };
        // onPreParse hook
        if (options.onPreParse !== undefined) {
            options.onPreParse(csv, options.state);
        }
        // parse the csv
        var headerLine = Csv.parsers.splitLines(csv, headerOptions);
        var headers = Csv.toArray(headerLine[0], options);
        // fetch the data
        lines = Csv.parsers.splitLines(csv, options);
        // reset the state for re-use
        options.state.colNum = 1;
        if (headers) {
            options.state.rowNum = 2;
        }
        else {
            options.state.rowNum = 1;
        }
        // convert data to objects
        for (var i = 0, len = lines.length; i < len; i++) {
            var entry = Csv.toArray(lines[i], options);
            if (options.transform !== undefined) {
                var obj = options.transform.call(undefined, entry);
                if (obj)
                    data.push(obj);
            }
            else {
                var object = {};
                for (var j = 0; j < headers.length; j++) {
                    object[headers[j]] = entry[j];
                }
                data.push(object);
            }
            // update row state
            options.state.rowNum++;
            options.state.colNum = 1;
        }
        // onPostParse hook
        if (options.onPostParse !== undefined) {
            options.onPostParse(data, options.state);
        }
        // push the value to a callback if one is defined
        if (!config.callback) {
            return data;
        }
        else {
            config.callback('', data);
        }
    };
    Csv.fromArrays = function (arrays, options, callback) {
        options = (options !== undefined ? options : {});
        var config = {};
        config.callback = ((callback !== undefined && typeof (callback) === 'function') ? callback : false);
        config.separator = 'separator' in options ? options.separator : Csv.defaults.separator;
        config.delimiter = 'delimiter' in options ? options.delimiter : Csv.defaults.delimiter;
        var output = '', line, lineValues, i, j;
        for (i = 0; i < arrays.length; i++) {
            line = arrays[i];
            lineValues = [];
            for (j = 0; j < line.length; j++) {
                var strValue = (line[j] === undefined || line[j] === null) ? '' : line[j].toString();
                if (strValue.indexOf(config.delimiter) > -1) {
                    strValue = strValue.replace(config.delimiter, config.delimiter + config.delimiter);
                }
                var escMatcher = '\n|\r|S|D';
                escMatcher = escMatcher.replace('S', config.separator);
                escMatcher = escMatcher.replace('D', config.delimiter);
                if (strValue.search(escMatcher) > -1) {
                    strValue = config.delimiter + strValue + config.delimiter;
                }
                lineValues.push(strValue);
            }
            output += lineValues.join(config.separator) + '\r\n';
        }
        // push the value to a callback if one is defined
        if (!config.callback) {
            return output;
        }
        else {
            config.callback('', output);
        }
    };
    Csv.fromObjects = function (objects, options, callback) {
        options = (options !== undefined ? options : {});
        var config = {};
        config.callback = ((callback !== undefined && typeof (callback) === 'function') ? callback : false);
        config.separator = 'separator' in options ? options.separator : Csv.defaults.separator;
        config.delimiter = 'delimiter' in options ? options.delimiter : Csv.defaults.delimiter;
        config.headers = 'headers' in options ? options.headers : Csv.defaults.headers;
        config.sortOrder = 'sortOrder' in options ? options.sortOrder : 'declare';
        config.manualOrder = 'manualOrder' in options ? options.manualOrder : [];
        config.transform = options.transform;
        if (typeof config.manualOrder === 'string') {
            config.manualOrder = Csv.toArray(config.manualOrder, config);
        }
        if (config.transform !== undefined) {
            var origObjects = objects;
            objects = [];
            var i;
            for (i = 0; i < origObjects.length; i++) {
                objects.push(config.transform.call(undefined, origObjects[i]));
            }
        }
        var props = Csv.helpers.collectPropertyNames(objects);
        if (config.sortOrder === 'alpha') {
            props.sort();
        } // else {} - nothing to do for 'declare' order
        if (config.manualOrder.length > 0) {
            var propsManual = [].concat(config.manualOrder);
            var p;
            for (p = 0; p < props.length; p++) {
                if (propsManual.indexOf(props[p]) < 0) {
                    propsManual.push(props[p]);
                }
            }
            props = propsManual;
        }
        var o, p, line, output = [], propName;
        if (config.headers) {
            output.push(props);
        }
        for (o = 0; o < objects.length; o++) {
            line = [];
            for (p = 0; p < props.length; p++) {
                propName = props[p];
                if (propName in objects[o] && typeof objects[o][propName] !== 'function') {
                    line.push(objects[o][propName]);
                }
                else {
                    line.push('');
                }
            }
            output.push(line);
        }
        // push the value to a callback if one is defined
        return Csv.fromArrays(output, options, config.callback);
    };
    Csv.defaults = (function () {
        function Defaults() {
        }
        Defaults.separator = ',';
        Defaults.delimiter = '"';
        Defaults.headers = true;
        return Defaults;
    })();
    Csv.hooks = (function () {
        function Hooks() {
        }
        Hooks.castToScalar = function (value, state) {
            var hasDot = /\./;
            if (isNaN(value)) {
                return value;
            }
            else {
                if (hasDot.test(value)) {
                    return parseFloat(value);
                }
                else {
                    var integer = parseInt(value);
                    if (isNaN(integer)) {
                        return null;
                    }
                    else {
                        return integer;
                    }
                }
            }
        };
        return Hooks;
    })();
    Csv.parsers = (function () {
        function Parsers() {
        }
        Parsers.parse = function (csv, options) {
            // cache settings
            var separator = options.separator;
            var delimiter = options.delimiter;
            // set initial state if it's missing
            if (!options.state.rowNum) {
                options.state.rowNum = 1;
            }
            if (!options.state.colNum) {
                options.state.colNum = 1;
            }
            // clear initial state
            var data = [];
            var entry = [];
            var state = 0;
            var value = '';
            var exit = false;
            function endOfEntry() {
                // reset the state
                state = 0;
                value = '';
                // if 'start' hasn't been met, don't output
                if (options.start && options.state.rowNum < options.start) {
                    // update global state
                    entry = [];
                    options.state.rowNum++;
                    options.state.colNum = 1;
                    return;
                }
                if (options.onParseEntry === undefined) {
                    // onParseEntry hook not set
                    data.push(entry);
                }
                else {
                    var hookVal = options.onParseEntry(entry, options.state); // onParseEntry Hook
                    // false skips the row, configurable through a hook
                    if (hookVal !== false) {
                        data.push(hookVal);
                    }
                }
                //console.log('entry:' + entry);
                // cleanup
                entry = [];
                // if 'end' is met, stop parsing
                if (options.end && options.state.rowNum >= options.end) {
                    exit = true;
                }
                // update global state
                options.state.rowNum++;
                options.state.colNum = 1;
            }
            function endOfValue() {
                if (options.onParseValue === undefined) {
                    // onParseValue hook not set
                    entry.push(value);
                }
                else {
                    var hook = options.onParseValue(value, options.state); // onParseValue Hook
                    // false skips the row, configurable through a hook
                    if (hook !== false) {
                        entry.push(hook);
                    }
                }
                //console.log('value:' + value);
                // reset the state
                value = '';
                state = 0;
                // update global state
                options.state.colNum++;
            }
            // escape regex-specific control chars
            var escSeparator = RegExp.escape(separator);
            var escDelimiter = RegExp.escape(delimiter);
            // compile the regEx str using the custom delimiter/separator
            var match = /(D|S|\r\n|\n|\r|[^DS\r\n]+)/;
            var matchSrc = match.source;
            matchSrc = matchSrc.replace(/S/g, escSeparator);
            matchSrc = matchSrc.replace(/D/g, escDelimiter);
            match = new RegExp(matchSrc, 'gm');
            // put on your fancy pants...
            // process control chars individually, use look-ahead on non-control chars
            csv.replace(match, function (m0) {
                if (exit) {
                    return;
                }
                switch (state) {
                    // the start of a value
                    case 0:
                        // null last value
                        if (m0 === separator) {
                            value += '';
                            endOfValue();
                            break;
                        }
                        // opening delimiter
                        if (m0 === delimiter) {
                            state = 1;
                            break;
                        }
                        // null last value
                        if (/^(\r\n|\n|\r)$/.test(m0)) {
                            endOfValue();
                            endOfEntry();
                            break;
                        }
                        // un-delimited value
                        value += m0;
                        state = 3;
                        break;
                    // delimited input
                    case 1:
                        // second delimiter? check further
                        if (m0 === delimiter) {
                            state = 2;
                            break;
                        }
                        // delimited data
                        value += m0;
                        state = 1;
                        break;
                    // delimiter found in delimited input
                    case 2:
                        // escaped delimiter?
                        if (m0 === delimiter) {
                            value += m0;
                            state = 1;
                            break;
                        }
                        // null value
                        if (m0 === separator) {
                            endOfValue();
                            break;
                        }
                        // end of entry
                        if (/^(\r\n|\n|\r)$/.test(m0)) {
                            endOfValue();
                            endOfEntry();
                            break;
                        }
                        // broken paser?
                        throw new Error('CSVDataError: Illegal State [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                    // un-delimited input
                    case 3:
                        // null last value
                        if (m0 === separator) {
                            endOfValue();
                            break;
                        }
                        // end of entry
                        if (/^(\r\n|\n|\r)$/.test(m0)) {
                            endOfValue();
                            endOfEntry();
                            break;
                        }
                        if (m0 === delimiter) {
                            // non-compliant data
                            throw new Error('CSVDataError: Illegal Quote [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                        }
                        // broken parser?
                        throw new Error('CSVDataError: Illegal Data [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                    default:
                        // shenanigans
                        throw new Error('CSVDataError: Unknown State [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                }
                //console.log('val:' + m0 + ' state:' + state);
            });
            // submit the last entry
            // ignore null last line
            if (entry.length !== 0) {
                endOfValue();
                endOfEntry();
            }
            return data;
        };
        Parsers.splitLines = function (csv, options) {
            // cache settings
            var separator = options.separator;
            var delimiter = options.delimiter;
            // set initial state if it's missing
            if (!options.state.rowNum) {
                options.state.rowNum = 1;
            }
            // clear initial state
            var entries = [];
            var state = 0;
            var entry = '';
            var exit = false;
            function endOfLine() {
                // reset the state
                state = 0;
                // if 'start' hasn't been met, don't output
                if (options.start && options.state.rowNum < options.start) {
                    // update global state
                    entry = '';
                    options.state.rowNum++;
                    return;
                }
                if (options.onParseEntry === undefined) {
                    // onParseEntry hook not set
                    entries.push(entry);
                }
                else {
                    var hookVal = options.onParseEntry(entry, options.state); // onParseEntry Hook
                    // false skips the row, configurable through a hook
                    if (hookVal !== false) {
                        entries.push(hookVal);
                    }
                }
                // cleanup
                entry = '';
                // if 'end' is met, stop parsing
                if (options.end && options.state.rowNum >= options.end) {
                    exit = true;
                }
                // update global state
                options.state.rowNum++;
            }
            // escape regex-specific control chars
            var escSeparator = RegExp.escape(separator);
            var escDelimiter = RegExp.escape(delimiter);
            // compile the regEx str using the custom delimiter/separator
            var match = /(D|S|\n|\r|[^DS\r\n]+)/;
            var matchSrc = match.source;
            matchSrc = matchSrc.replace(/S/g, escSeparator);
            matchSrc = matchSrc.replace(/D/g, escDelimiter);
            match = new RegExp(matchSrc, 'gm');
            // put on your fancy pants...
            // process control chars individually, use look-ahead on non-control chars
            csv.replace(match, function (m0) {
                if (exit) {
                    return;
                }
                switch (state) {
                    // the start of a value/entry
                    case 0:
                        // null value
                        if (m0 === separator) {
                            entry += m0;
                            state = 0;
                            break;
                        }
                        // opening delimiter
                        if (m0 === delimiter) {
                            entry += m0;
                            state = 1;
                            break;
                        }
                        // end of line
                        if (m0 === '\n') {
                            endOfLine();
                            break;
                        }
                        // phantom carriage return
                        if (/^\r$/.test(m0)) {
                            break;
                        }
                        // un-delimit value
                        entry += m0;
                        state = 3;
                        break;
                    // delimited input
                    case 1:
                        // second delimiter? check further
                        if (m0 === delimiter) {
                            entry += m0;
                            state = 2;
                            break;
                        }
                        // delimited data
                        entry += m0;
                        state = 1;
                        break;
                    // delimiter found in delimited input
                    case 2:
                        // escaped delimiter?
                        var prevChar = entry.substr(entry.length - 1);
                        if (m0 === delimiter && prevChar === delimiter) {
                            entry += m0;
                            state = 1;
                            break;
                        }
                        // end of value
                        if (m0 === separator) {
                            entry += m0;
                            state = 0;
                            break;
                        }
                        // end of line
                        if (m0 === '\n') {
                            endOfLine();
                            break;
                        }
                        // phantom carriage return
                        if (m0 === '\r') {
                            break;
                        }
                        // broken paser?
                        throw new Error('CSVDataError: Illegal state [Row:' + options.state.rowNum + ']');
                    // un-delimited input
                    case 3:
                        // null value
                        if (m0 === separator) {
                            entry += m0;
                            state = 0;
                            break;
                        }
                        // end of line
                        if (m0 === '\n') {
                            endOfLine();
                            break;
                        }
                        // phantom carriage return
                        if (m0 === '\r') {
                            break;
                        }
                        // non-compliant data
                        if (m0 === delimiter) {
                            throw new Error('CSVDataError: Illegal quote [Row:' + options.state.rowNum + ']');
                        }
                        // broken parser?
                        throw new Error('CSVDataError: Illegal state [Row:' + options.state.rowNum + ']');
                    default:
                        // shenanigans
                        throw new Error('CSVDataError: Unknown state [Row:' + options.state.rowNum + ']');
                }
                //console.log('val:' + m0 + ' state:' + state);
            });
            // submit the last entry
            // ignore null last line
            if (entry !== '') {
                endOfLine();
            }
            return entries;
        };
        Parsers.parseEntry = function (csv, options) {
            // cache settings
            var separator = options.separator;
            var delimiter = options.delimiter;
            // set initial state if it's missing
            if (!options.state.rowNum) {
                options.state.rowNum = 1;
            }
            if (!options.state.colNum) {
                options.state.colNum = 1;
            }
            // clear initial state
            var entry = [];
            var state = 0;
            var value = '';
            function endOfValue() {
                if (options.onParseValue === undefined) {
                    // onParseValue hook not set
                    entry.push(value);
                }
                else {
                    var hook = options.onParseValue(value, options.state); // onParseValue Hook
                    // false skips the value, configurable through a hook
                    if (hook !== false) {
                        entry.push(hook);
                    }
                }
                // reset the state
                value = '';
                state = 0;
                // update global state
                options.state.colNum++;
            }
            // checked for a cached regEx first
            if (!options.match) {
                // escape regex-specific control chars
                var escSeparator = RegExp.escape(separator);
                var escDelimiter = RegExp.escape(delimiter);
                // compile the regEx str using the custom delimiter/separator
                var match = /(D|S|\n|\r|[^DS\r\n]+)/;
                var matchSrc = match.source;
                matchSrc = matchSrc.replace(/S/g, escSeparator);
                matchSrc = matchSrc.replace(/D/g, escDelimiter);
                options.match = new RegExp(matchSrc, 'gm');
            }
            // put on your fancy pants...
            // process control chars individually, use look-ahead on non-control chars
            csv.replace(options.match, function (m0) {
                switch (state) {
                    // the start of a value
                    case 0:
                        // null last value
                        if (m0 === separator) {
                            value += '';
                            endOfValue();
                            break;
                        }
                        // opening delimiter
                        if (m0 === delimiter) {
                            state = 1;
                            break;
                        }
                        // skip un-delimited new-lines
                        if (m0 === '\n' || m0 === '\r') {
                            break;
                        }
                        // un-delimited value
                        value += m0;
                        state = 3;
                        break;
                    // delimited input
                    case 1:
                        // second delimiter? check further
                        if (m0 === delimiter) {
                            state = 2;
                            break;
                        }
                        // delimited data
                        value += m0;
                        state = 1;
                        break;
                    // delimiter found in delimited input
                    case 2:
                        // escaped delimiter?
                        if (m0 === delimiter) {
                            value += m0;
                            state = 1;
                            break;
                        }
                        // null value
                        if (m0 === separator) {
                            endOfValue();
                            break;
                        }
                        // skip un-delimited new-lines
                        if (m0 === '\n' || m0 === '\r') {
                            break;
                        }
                        // broken paser?
                        throw new Error('CSVDataError: Illegal State [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                    // un-delimited input
                    case 3:
                        // null last value
                        if (m0 === separator) {
                            endOfValue();
                            break;
                        }
                        // skip un-delimited new-lines
                        if (m0 === '\n' || m0 === '\r') {
                            break;
                        }
                        // non-compliant data
                        if (m0 === delimiter) {
                            throw new Error('CSVDataError: Illegal Quote [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                        }
                        // broken parser?
                        throw new Error('CSVDataError: Illegal Data [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                    default:
                        // shenanigans
                        throw new Error('CSVDataError: Unknown State [Row:' + options.state.rowNum + '][Col:' + options.state.colNum + ']');
                }
                //console.log('val:' + m0 + ' state:' + state);
            });
            // submit the last value
            endOfValue();
            return entry;
        };
        return Parsers;
    })();
    Csv.helpers = (function () {
        function Helpers() {
        }
        Helpers.collectPropertyNames = function (objects) {
            var o, propName, props = [];
            for (o in objects) {
                for (propName in objects[o]) {
                    if ((objects[o].hasOwnProperty(propName)) &&
                        (props.indexOf(propName) < 0) &&
                        (typeof objects[o][propName] !== 'function')) {
                        props.push(propName);
                    }
                }
            }
            return props;
        };
        return Helpers;
    })();
    return Csv;
})();
RegExp.escape = function (s) {
    return s.replace(/[-\/\\^$*+?.()|[\]{}]/g, '\\$&');
};
