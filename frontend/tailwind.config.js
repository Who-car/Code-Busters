export default {
  prefix: 'tw-',
  content: [
    "./index.html",
    "./src/**/*.{vue,js,ts,jsx,tsx}",
  ],
  theme: {
    extend: {
      colors: {
        'base-background': '#00454b',
        'secondary-background': '#263238',
        'base-orange': '#FA8D12',
        'base-yellow': '#FAC710'
      },
      fontFamily: {
        'base-ui': ['Visitor Rus', 'sans'],
        'base-text': ['Disket Mono', 'sans']
      },
    },
    screens: {
      'sm': '640px',
      'md': '825px',
      'lg': '1024px',
      'xl': '1280px',
      '2xl': '1536px'
    }
  },
  variants: {},
  plugins: [],
}