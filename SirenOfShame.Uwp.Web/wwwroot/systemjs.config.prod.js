/**
 * PLUNKER VERSION (based on systemjs.config.js in angular.io)
 * System configuration for Angular 2 samples
 * Adjust as necessary for your application needs.
 */
(function(global) {
    var ngPackageNames = [
        'common',
        'compiler',
        'core',
        'http',
        'platform-browser',
        'platform-browser-dynamic',
        'router',
        'upgrade'
    ];

    var packages = {
        app: {
            defaultExtension: 'js',
            format: 'register'
        }
    };

    ngPackageNames.forEach(function(pkgName) {
        packages['@angular/' + pkgName] = { main: pkgName + '.umd.js', defaultExtension: 'js' };
    });

    var config = {
        packages: packages
    };
    System.config(config);
})(this);