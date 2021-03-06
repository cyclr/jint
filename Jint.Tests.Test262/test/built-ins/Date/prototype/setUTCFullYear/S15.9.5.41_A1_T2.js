// Copyright 2009 the Sputnik authors.  All rights reserved.
// This code is governed by the BSD license found in the LICENSE file.

/*---
info: The Date.prototype property "setUTCFullYear" has { DontEnum } attributes
esid: sec-date.prototype.setutcfullyear
es5id: 15.9.5.41_A1_T2
description: Checking absence of DontDelete attribute
---*/

if (delete Date.prototype.setUTCFullYear === false) {
  $ERROR('#1: The Date.prototype.setUTCFullYear property has not the attributes DontDelete');
}

if (Date.prototype.hasOwnProperty('setUTCFullYear')) {
  $ERROR('#2: The Date.prototype.setUTCFullYear property has not the attributes DontDelete');
}
