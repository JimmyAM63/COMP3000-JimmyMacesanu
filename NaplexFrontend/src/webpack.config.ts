const path = require('path');

module.exports = {
  entry: './src/index.ts', // Your TypeScript entry point
  output: {
    filename: 'bundle.js',
    path: path.resolve(__dirname, 'dist')
  },
  module: {
    rules: [
      // TypeScript Rule
      {
        test: /\.tsx?$/,
        use: 'ts-loader',
        exclude: /node_modules/,
      },
      // SASS Rule
      {
        test: /\.s[ac]ss$/i,
        use: [
          'style-loader',
          'css-loader',
          'sass-loader'
        ],
      }
    ],
  },
  resolve: {
    extensions: ['.tsx', '.ts', '.js', '.scss', '.sass'],
  },
};
