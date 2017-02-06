var azureMobileApps = require('azure-mobile-apps');

var table = azureMobileApps.table();

// Additional configuration for the table goes here

table.autoIncrement = true;

table.read.access = 'anonymous';
table.insert.access = 'anonymous';
table.update.access = 'anonymous';
table.delete.access = 'anonymous';

module.exports = table;