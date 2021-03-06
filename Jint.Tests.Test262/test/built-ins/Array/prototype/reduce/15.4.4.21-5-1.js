// Copyright (c) 2012 Ecma International.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
esid: sec-array.prototype.reduce
es5id: 15.4.4.21-5-1
description: >
    Array.prototype.reduce throws TypeError if 'length' is 0 (empty
    array), no initVal
---*/

function cb() {}
assert.throws(TypeError, function() {
  [].reduce(cb);
});
