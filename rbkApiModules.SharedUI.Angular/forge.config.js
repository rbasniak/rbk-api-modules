module.exports = {
  packagerConfig: {
    asar: true,
    platform: ['win32', 'darwin'],
    // icon: 'src/assets/icons/mac/icon.icns',
    icon: 'src/assets/icons/win/icon.ico',
    // icon: 'src/assets/icons/png/512x512.png',
    arch: ['x64'],
    usageDescription: {
      Camera: 'Necessário para registro fotográfico das marcações.',
    },
    ignore: [
      '^(\/e2e$)',
      '^(\/app$)',
      '^(\/src$)',
      '^(\/node_modules$)',
      '^(\/out$)',
      '^(\/out-tsc$)',
      '^(\/scripts$)',
      '^(\/styles$)',
      '^(\/themes$)',
      '.editorconfig',
      '.gitignore',
      'angular.json',
      'package-lock.json',
      '^(\/README.md$)',
      'tsconfig.json',
      'tslint.json',
      'tsconfig.app.json',
      'tsconfig.spec.json',
      'karma.conf.js',
      'forge.config.js',
      'electron-builder.json',
      '^(\/.vscode$)',
      '.browserslistrc',
      '.eslintignore',
      '.eslintrc.js'
    ]
  },
  makers: [
    {
      name: '@electron-forge/maker-zip',
      platforms: ['win32', 'darwin'],
      config: {
      }
    },
  ]
};