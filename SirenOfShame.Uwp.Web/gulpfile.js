/// <binding Clean='clean' ProjectOpened='watch' />
"use strict";

var gulp = require("gulp"),
    rimraf = require("rimraf"),
    concat = require("gulp-concat"),
    cssmin = require("gulp-cssmin"),
    uglify = require("gulp-uglify"),
    ts = require('gulp-typescript'),
    htmlreplace = require('gulp-html-replace');

var paths = {
    webroot: "./wwwroot/"
};

paths.js = paths.webroot + "js/**/*.js";
paths.minJs = paths.webroot + "js/**/*.min.js";
paths.css = paths.webroot + "css/**/*.css";
paths.minCss = paths.webroot + "css/**/*.min.css";
paths.concatJsDest = paths.webroot + "js/site.min.js";
paths.concatCssDest = paths.webroot + "css/site.min.css";

gulp.task("clean:js", function (cb) {
    rimraf(paths.concatJsDest, cb);
});

gulp.task("clean:css", function (cb) {
    rimraf(paths.concatCssDest, cb);
});

gulp.task("clean:dist", function(cb) {
    rimraf('./wwwroot/dist', cb);
});

gulp.task("clean", ["clean:js", "clean:css", "clean:dist"]);

gulp.task("min:css", function () {
    return gulp.src([paths.css, "!" + paths.minCss])
        .pipe(concat(paths.concatCssDest))
        .pipe(cssmin())
        .pipe(gulp.dest("."));
});

gulp.task("min", ["min:css"]);

gulp.task("rebuild:vendorcopy", function () {
    gulp.src([
            'es6-shim/es6-shim.min.js',
            'systemjs/dist/system-polyfills.js',
            'systemjs/dist/system.src.js',
            'reflect-metadata/Reflect.js',
            'rxjs/**',
            'zone.js/dist/**',
            '@angular/**'
    ], {
        cwd: "node_modules/**"
    })
    .pipe(gulp.dest("./wwwroot/lib"));
});

gulp.task("pideploy:copy", ["deploy:ts"], function() {
    var dst = '../SirenOfShame.Uwp.Background/wwwroot/';
    gulp.src([
        paths.webroot + 'css/*',
        paths.webroot + 'images/*',
        paths.webroot + 'lib/bootstrap/dist/css/bootstrap.min.css',
        paths.webroot + 'lib/bootstrap/dist/js/bootstrap.min.js',
        paths.webroot + 'lib/jquery/dist/jquery.min.js',
        paths.webroot + 'components/*.html'
    ], { base: 'wwwroot' })
        .pipe(gulp.dest(dst));

    gulp.src([
        paths.webroot + 'dist/index.html',
        paths.webroot + 'dist/**/*.js'
    ], { base: paths.webroot + "dist" })
        .pipe(gulp.dest(dst));
});

gulp.task("pideploy", ["deploy", "pideploy:copy"]);

gulp.task('ts', function (done) {
    var tsProject = ts.createProject('./Ts/tsconfig.json');
    var tsResult = gulp.src([
            "Ts/**/*.ts"
    ])
    .pipe(ts(tsProject), undefined, ts.reporter.fullReporter());
    return tsResult.js.pipe(gulp.dest('./wwwroot/appScripts'));
});

gulp.task('debug:copyts',
    function(done) {
        return gulp.src(["Ts/**/*.ts"])
            .pipe(gulp.dest("./wwwroot/Ts"));
    });

gulp.task('watch', ['watch.ts', 'debug:copyts']);

gulp.task('watch.ts', ['ts'], function () {
    return gulp.watch('Ts/*.ts', ['ts']);
});

gulp.task('deploy:ts', function (done) {
    var tsProject = ts.createProject('Ts/tsconfig.json',
    {
        typescript: require('typescript'),
        outFile: '../wwwroot/appScripts/app.js',
        inlineSourceMap: true,
        declaration: false,
        stripInternal: true,
        module: 'system',
        noEmitOnError: false,
        rootDir: '.',
        inlineSources: true
    });

    var tsResult = gulp.src([
            "Ts/*.ts"
    ])
        .pipe(ts(tsProject), undefined, ts.reporter.fullReporter());

    return tsResult.js
        .pipe(concat('app.min.js'))
        .pipe(uglify())
        .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('deploy:vendor-bundle', function () {
    gulp.src([
            'es6-shim/es6-shim.min.js',
            'systemjs/dist/system-polyfills.js',
            'zone.js/dist/zone.js',
            'systemjs/dist/system.src.js',
            'rxjs/bundles/Rx.js',
            'reflect-metadata/Reflect.js'
    ], {
        cwd: "node_modules/**"
    })
    .pipe(concat('vendors.min.js'))
    .pipe(uglify())
    .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('deploy:ng2-bundle', function () {
    gulp.src([
      'node_modules/@angular/common/bundles/common.umd.min.js',
      'node_modules/@angular/compiler/bundles/compiler.umd.min.js',
      'node_modules/@angular/core/bundles/core.umd.min.js',
      'node_modules/@angular/forms/bundles/forms.umd.min.js',
      'node_modules/@angular/http/bundles/http.umd.min.js',
      'node_modules/@angular/platform-browser/bundles/platform-browser.umd.min.js',
      'node_modules/@angular/platform-browser-dynamic/bundles/platform-browser-dynamic.umd.min.js',
      'node_modules/@angular/router/bundles/router.umd.min.js'
    ], { base: 'node_modules' })
    .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('deploy:boot-bundle', function () {
    gulp.src('./wwwroot/config.prod.js')
      .pipe(concat('boot.min.js'))
      .pipe(uglify())
      .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('deploy:systemconfig-bundle', function () {
    gulp.src('./wwwroot/systemjs.config.prod.js')
      .pipe(concat('systemjs.config.js'))
      .pipe(uglify())
      .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('deploy:html', function () {
    gulp.src('./wwwroot/index.html')
      .pipe(htmlreplace({
          'vendor': 'vendors.min.js',
          'app': 'app.min.js',
          'boot': 'boot.min.js'
      }))
      .pipe(gulp.dest('./wwwroot/dist'));
});

gulp.task('deploy', ['deploy:ts', 'deploy:vendor-bundle', 'deploy:ng2-bundle', 'deploy:boot-bundle', 'deploy:systemconfig-bundle', 'deploy:html']);
