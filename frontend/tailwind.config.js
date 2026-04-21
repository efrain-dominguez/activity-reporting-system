/** @type {import('tailwindcss').Config} */
module.exports = {
  content: [
    "./src/**/*.{html,ts}",
    "./src/assets/lineone/**/*.{html,js}"
  ],
  darkMode: 'class',
  theme: {
    extend: {
      colors: {
        primary: {
          DEFAULT: '#4f46e5',
          light: '#6366f1',
          focus: '#4338ca',
        },
        accent: {
          DEFAULT: '#5f5af6',
          light: '#a3a0f4',
          focus: '#4f46e5',
        },
        secondary: {
          DEFAULT: '#f000b9',
          light: '#f772e1',
          focus: '#d600a1',
        },
        info: {
          DEFAULT: '#0ea5e9',
          focus: '#0284c7',
        },
        success: {
          DEFAULT: '#10b981',
          focus: '#059669',
        },
        warning: {
          DEFAULT: '#f59e0b',
          focus: '#d97706',
        },
        error: {
          DEFAULT: '#ef4444',
          focus: '#dc2626',
        },
        slate: {
          50: '#f8fafc',
          100: '#f1f5f9',
          150: '#e9eef5',
          200: '#e2e8f0',
          300: '#cbd5e1',
          400: '#94a3b8',
          500: '#64748b',
          600: '#475569',
          700: '#334155',
          800: '#1e293b',
          900: '#0f172a',
        },
        navy: {
          50: '#f6f8fb',
          100: '#e9eef5',
          200: '#dbe4f0',
          300: '#b8cade',
          400: '#7b96b2',
          450: '#6882a1',
          500: '#4a5f7f',
          600: '#3c4d63',
          700: '#2f3f52',
          750: '#26334a',
          800: '#1d283a',
          900: '#0f1729',
        },
      },
      fontFamily: {
        sans: ['Inter', 'sans-serif'],
        inter: ['Inter', 'sans-serif'],
      },
      fontSize: {
        'tiny': '0.625rem',
        'tiny-plus': '0.6875rem',
        'xs': '0.75rem',
        'xs-plus': '0.8125rem',
      },
      spacing: {
        '4.5': '1.125rem',
        '5.5': '1.375rem',
        '18': '4.5rem',
      },
      minHeight: {
        '100vh': '100vh',
      },
      zIndex: {
        '1': '1',
        '100': '100',
        '150': '150',
        '151': '151',
      },
      boxShadow: {
        'soft': '0 3px 10px 0 rgba(48, 46, 56, 0.06)',
        'xs': '0 1px 2px 0 rgba(0, 0, 0, 0.05)',
        'sm': '0 2px 4px 0 rgba(48, 46, 56, 0.08)',
        'base': '0 5px 20px 0 rgba(48, 46, 56, 0.12)',
        'lg': '0 10px 40px 0 rgba(48, 46, 56, 0.12)',
        'xl': '0 15px 50px 0 rgba(48, 46, 56, 0.12)',
        '2xl': '0 25px 60px 0 rgba(0, 0, 0, 0.15)',
      },
      outline: {
        hidden: 'none',
      }
    },
  },
  plugins: [
    function({ addUtilities }) {
      addUtilities({
        '.outline-hidden': {
          outline: 'none',
        },
      })
    }
  ],
}
