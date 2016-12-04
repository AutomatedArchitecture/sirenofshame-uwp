/**
 * PLUNKER VERSION (based on systemjs.config.js in angular.io)
 * System configuration for Angular 2 samples
 * Adjust as necessary for your application needs.
 */
(function(global) {
    // map tells the System loader where to look for things
    var map = {
        'rxjs': 'https://npmcdn.com/rxjs@5.0.0-beta.12', // <- BAD, should be 'lib/rxjs' so we can support 100% offline mode, but stupid ng2 won't support single rxjs umd file at present, see http://stackoverflow.com/questions/38793349/angular2-systemjs-failed-to-use-rx-umd-js
    };

    var ngPackageNames = [
        'common',
        'compiler',
        'core',
        'forms',
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
        packages['@angular/' + pkgName] = { main: 'bundles/' + pkgName + '.umd.min.js', defaultExtension: 'js' };
    });

    var config = {
        map: map,
        packages: packages
    };
    System.config(config);
})(this);