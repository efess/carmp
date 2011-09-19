#include "Utils.h"

using namespace std;

Utils::Utils(void)
{
}


Utils::~Utils(void)
{
}

/* Doesn't work lol */


void *Utils::file_contents(const char *filename, GLint *length)
{
    FILE *f = fopen(filename, "r");
    void *buffer;

    if (!f) {
        fprintf(stderr, "Unable to open %s for reading\n", filename);
        return NULL;
    }

    fseek(f, 0, SEEK_END);
    *length = ftell(f);
    fseek(f, 0, SEEK_SET);

    buffer = malloc(*length+1);
    *length = fread(buffer, 1, *length, f);
    fclose(f);
    ((char*)buffer)[*length] = '\0';

    return buffer;
}

//void Utils::ShowInfoLog(
//    GLuint object,
//    PFNGLGETSHADERIVPROC glGet__iv,
//    PFNGLGETSHADERINFOLOGPROC glGet__InfoLog
//)
//{
//    GLint log_length;
//    char *log;
//
//    glGet__iv(object, GL_INFO_LOG_LENGTH, &log_length);
//    log = (char*)malloc(log_length);
//    glGet__InfoLog(object, log_length, NULL, log);
//    fprintf(stderr, "%s", log);
//	
//	fflush(stderr);
//
//    free(log);
//}
