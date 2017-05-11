/// <binding BeforeBuild='default' />
//*********** IMPORTS *****************
'use strict';

var gulp = require('gulp');
var sass = require('gulp-sass');
var rename = require('gulp-rename');
var gutil = require('gulp-util');
var concat = require("gulp-concat");
var uglify = require('gulp-uglify');
var insert = require('gulp-insert');

var src = 'src/Shield'
var jsOutput = 'App_Plugins/Shield';
var jsFiles = [
    {
	    subFiles: [
            src + '.UmbracoAccess/Js/*.js',
            src + '.UmbracoAccess/PropertyEditors/IpAddress/Js/*.js'
	    ],
        output: src + '.UmbracoAccess/' + jsOutput + '.UmbracoAccess/js',
	    name: 'UmbracoAccess.js',
	    nameMin: 'UmbracoAccess.min.js'
    },
    {
        subFiles: [
            src + '.MediaProtection/Js/*.js'
        ],
        output: src + '.MediaProtection/' + jsOutput + '.MediaProtection/js',
        name: 'MediaProtection.js',
        nameMin: 'MediaProtection.min.js'
    }
];

var cssOutput = 'App_Plugins/Shield';
var scssFiles = [
    {
        files: src + '.UmbracoAccess/Scss/*.scss',
        scss: src + '.UmbracoAccess/Scss/UmbracoAccess.scss',
        output: src + '.UmbracoAccess/' + cssOutput + '.UmbracoAccess/Css',
        name: 'UmbracoAccess.css',
        nameMin: 'UmbracoAccess.min.css'
    }
];

gulp.task('default', ['buildjs', 'buildscss']);

gulp.task('buildjs', function () {
    for (var j in jsFiles) {
        buildscript(jsFiles[j]);
    }
});

gulp.task('buildscss', function () {
    for (var j in scssFiles) {
        buildscss(scssFiles[j]);
    }
});

function buildscript (jsData) {
    gulp.src(jsData.subFiles)
    .pipe(concat(jsData.name))
    .pipe(insert.wrap('(function(root){ \n', '\n }(window));'))
    .pipe(gulp.dest(jsData.output));
}

function buildscss(sassData) {
    gulp.src(sassData.files)
    .pipe(sass())
    .pipe(rename(sassData.name))
	.pipe(gulp.dest(sassData.output))
}
