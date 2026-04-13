// ===================== SCROLL REVEAL ANIMATIONS =====================
const scrollReveal = () => {
  const reveals = document.querySelectorAll('.scroll-reveal');
  
  const revealOnScroll = () => {
    reveals.forEach(element => {
      const elementTop = element.getBoundingClientRect().top;
      const elementVisible = 100;
      
      if (elementTop < window.innerHeight - elementVisible) {
        element.classList.add('revealed');
      }
    });
  };
  
  window.addEventListener('scroll', revealOnScroll);
  revealOnScroll(); // Initial check
};

// ===================== HEADER SCROLL EFFECT =====================
const headerScrollEffect = () => {
  const header = document.querySelector('.header');
  let lastScroll = 0;
  
  window.addEventListener('scroll', () => {
    const currentScroll = window.pageYOffset;
    
    if (currentScroll > 100) {
      header.classList.add('scrolled');
    } else {
      header.classList.remove('scrolled');
    }
    
    lastScroll = currentScroll;
  });
};

// ===================== SLIDER =====================
const slides = document.querySelectorAll('.slide');
const dotsContainer = document.getElementById('sliderDots');
const prevBtn = document.getElementById('prevBtn');
const nextBtn = document.getElementById('nextBtn');
let currentSlide = 0;
let autoSlideTimer;
let isSliding = false;

// Tạo dots
slides.forEach((_, i) => {
  const dot = document.createElement('div');
  dot.classList.add('dot');
  if (i === 0) dot.classList.add('active');
  dot.addEventListener('click', () => goToSlide(i));
  dotsContainer.appendChild(dot);
});

const dots = document.querySelectorAll('.dot');

function goToSlide(index) {
  if (isSliding) return;
  isSliding = true;
  
  slides[currentSlide].classList.remove('active');
  dots[currentSlide].classList.remove('active');
  currentSlide = (index + slides.length) % slides.length;
  slides[currentSlide].classList.add('active');
  dots[currentSlide].classList.add('active');
  
  setTimeout(() => { isSliding = false; }, 800);
}

function nextSlide() { goToSlide(currentSlide + 1); }
function prevSlide() { goToSlide(currentSlide - 1); }

function startAutoSlide() {
  autoSlideTimer = setInterval(nextSlide, 6000);
}
function resetAutoSlide() {
  clearInterval(autoSlideTimer);
  startAutoSlide();
}

prevBtn.addEventListener('click', () => { prevSlide(); resetAutoSlide(); });
nextBtn.addEventListener('click', () => { nextSlide(); resetAutoSlide(); });

// Touch/Swipe support with improved sensitivity
let touchStartX = 0;
let touchEndX = 0;
const sliderEl = document.querySelector('.hero-slider');

sliderEl.addEventListener('touchstart', e => { 
  touchStartX = e.touches[0].clientX; 
}, { passive: true });

sliderEl.addEventListener('touchend', e => {
  touchEndX = e.changedTouches[0].clientX;
  const diff = touchStartX - touchEndX;
  
  if (Math.abs(diff) > 50) { 
    diff > 0 ? nextSlide() : prevSlide(); 
    resetAutoSlide(); 
  }
}, { passive: true });

// Keyboard navigation
document.addEventListener('keydown', e => {
  if (e.key === 'ArrowLeft') {
    prevSlide();
    resetAutoSlide();
  } else if (e.key === 'ArrowRight') {
    nextSlide();
    resetAutoSlide();
  }
});

startAutoSlide();

// ===================== HAMBURGER MENU =====================
const hamburger = document.getElementById('hamburger');
const navbar = document.querySelector('.navbar');
const body = document.body;

hamburger.addEventListener('click', () => {
  navbar.classList.toggle('open');
  hamburger.classList.toggle('active');
  body.style.overflow = navbar.classList.contains('open') ? 'hidden' : '';
});

// Đóng menu khi click vào link
document.querySelectorAll('.navbar a').forEach(link => {
  link.addEventListener('click', () => {
    navbar.classList.remove('open');
    hamburger.classList.remove('active');
    body.style.overflow = '';
  });
});

// Đóng menu khi click outside
document.addEventListener('click', (e) => {
  if (!navbar.contains(e.target) && !hamburger.contains(e.target) && navbar.classList.contains('open')) {
    navbar.classList.remove('open');
    hamburger.classList.remove('active');
    body.style.overflow = '';
  }
});

// ===================== BACK TO TOP =====================
const backToTopBtn = document.getElementById('backToTop');
let scrollTimeout;

window.addEventListener('scroll', () => {
  const scrollY = window.scrollY;
  
  if (scrollY > 300) {
    backToTopBtn.classList.add('show');
  } else {
    backToTopBtn.classList.remove('show');
  }
  
  // Hide button while scrolling, show after scroll ends
  backToTopBtn.style.opacity = '0.7';
  clearTimeout(scrollTimeout);
  scrollTimeout = setTimeout(() => {
    backToTopBtn.style.opacity = '1';
  }, 150);
}, { passive: true });

backToTopBtn.addEventListener('click', () => {
  window.scrollTo({ 
    top: 0, 
    behavior: 'smooth' 
  });
});

// ===================== SMOOTH HOVER SOLUTIONS =====================
document.querySelectorAll('.solutions-list li').forEach((item, index) => {
  // Stagger animation delay
  item.style.transitionDelay = `${index * 0.05}s`;
  
  item.addEventListener('click', () => {
    // Add ripple effect
    const ripple = document.createElement('span');
    ripple.style.cssText = `
      position: absolute;
      width: 10px;
      height: 10px;
      background: rgba(21, 101, 192, 0.3);
      border-radius: 50%;
      pointer-events: none;
      animation: ripple 0.6s ease-out;
    `;
    
    item.style.position = 'relative';
    item.appendChild(ripple);
    
    setTimeout(() => ripple.remove(), 600);
  });
});

// Add ripple animation
const style = document.createElement('style');
style.textContent = `
  @keyframes ripple {
    from {
      transform: scale(0);
      opacity: 1;
    }
    to {
      transform: scale(10);
      opacity: 0;
    }
  }
`;
document.head.appendChild(style);

// ===================== PARALLAX EFFECT =====================
const addParallaxEffect = () => {
  const slides = document.querySelectorAll('.slide');
  
  window.addEventListener('scroll', () => {
    const scrolled = window.pageYOffset;
    slides.forEach(slide => {
      if (slide.classList.contains('active')) {
        const speed = 0.5;
        slide.style.transform = `translateY(${scrolled * speed}px)`;
      }
    });
  }, { passive: true });
};

// ===================== PROJECT CARD TILT EFFECT =====================
const addTiltEffect = () => {
  const cards = document.querySelectorAll('.project-card, .news-featured');
  
  cards.forEach(card => {
    card.addEventListener('mousemove', (e) => {
      const rect = card.getBoundingClientRect();
      const x = e.clientX - rect.left;
      const y = e.clientY - rect.top;
      
      const centerX = rect.width / 2;
      const centerY = rect.height / 2;
      
      const rotateX = (y - centerY) / 20;
      const rotateY = (centerX - x) / 20;
      
      card.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-8px)`;
    });
    
    card.addEventListener('mouseleave', () => {
      card.style.transform = '';
    });
  });
};

// ===================== INITIALIZE ALL EFFECTS =====================
document.addEventListener('DOMContentLoaded', () => {
  scrollReveal();
  headerScrollEffect();
  addParallaxEffect();
  addTiltEffect();
  
  // Add smooth scroll for all anchor links
  document.querySelectorAll('a[href^="#"]').forEach(anchor => {
    anchor.addEventListener('click', function(e) {
      const href = this.getAttribute('href');
      if (href !== '#' && document.querySelector(href)) {
        e.preventDefault();
        document.querySelector(href).scrollIntoView({
          behavior: 'smooth'
        });
      }
    });
  });
  
  console.log('🚀 SG Energy - UI/UX Enhanced!');
});
// Hàm load dữ liệu từ file JSON
async function loadData() {
  const res = await fetch('data.json');
  const data = await res.json();

  renderServices(data.services);
  renderProducts(data.products);
  renderProjects(data.projects);
}

// Render danh sách dịch vụ
function renderServices(services) {
  const container = document.getElementById('services-list');
  container.innerHTML = services.map(s => `
    <div class="service-card">
      <i class="fas ${s.icon}"></i>
      <h3>${s.title}</h3>
      <p>${s.description}</p>
    </div>
  `).join('');
}

// Render danh sách sản phẩm
function renderProducts(products) {
  const container = document.getElementById('products-list');
  container.innerHTML = products.map(p => `
    <div class="product-card">
      <img src="${p.image}" alt="${p.name}" />
      <h4>${p.name}</h4>
      <span class="price">${p.price}</span>
      <span class="brand">${p.brand}</span>
    </div>
  `).join('');
}

// Render danh sách dự án
function renderProjects(projects) {
  const container = document.getElementById('projects-list');
  container.innerHTML = projects.map(p => `
    <div class="project-card">
      <img src="${p.image}" alt="${p.title}" />
      <div class="project-caption">
        <h4>${p.title}</h4>
        <p>Công suất: ${p.capacity} – Năm: ${p.year}</p>
      </div>
    </div>
  `).join('');
}

// Gọi khi trang load
loadData();