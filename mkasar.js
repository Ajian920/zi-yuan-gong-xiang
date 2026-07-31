const path = require('path');
const projectDir = 'E:\\Codex项目\\资源共享';
const asar = require(path.join(projectDir, 'node_modules', '@electron', 'asar'));
const fs = require('fs');
const src = process.argv[2];
const dest = process.argv[3];
console.log('src:', src, 'exists:', fs.existsSync(src));
console.log('dest:', dest);
if (!fs.existsSync(src)) {
    console.error('Source not found');
    process.exit(1);
}
try {
    asar.createPackage(src, dest).then(() => {
        console.log('OK ' + fs.statSync(dest).size);
    }).catch(e => {
        console.error('ERR ' + e.message);
    });
} catch(e) {
    console.error('ERR ' + e.message);
}